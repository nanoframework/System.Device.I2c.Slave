// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Device.I2c
{
    /// <summary>
    /// Represents an I2C slave device.
    /// </summary>
    public class I2cSlaveDevice : IDisposable
    {
        private readonly int _busId;
        private readonly int _deviceAddress;

        [Diagnostics.DebuggerBrowsable(Diagnostics.DebuggerBrowsableState.Never)]
        private bool _disposed;

        // this is used as the lock object 
        // a lock is required because multiple threads can access the device
        [Diagnostics.DebuggerBrowsable(Diagnostics.DebuggerBrowsableState.Never)]
        private readonly object _syncLock;

        // speeds up the execution of ReadByte and WriteByte operations
        [Diagnostics.DebuggerBrowsable(Diagnostics.DebuggerBrowsableState.Never)]
        private readonly byte[] _buffer;

        /// <summary>
        /// The address of the I2C slave device.
        /// </summary>
        /// <value> The address of the I2C device.</value>
        public int DeviceAddress => _deviceAddress;

        /// <summary>
        /// The bus ID the I2C slave device is available.
        /// </summary>
        public int BusId { get => _busId; }

        /// <summary>
        /// Create an I2C slave device with the specified address.
        /// </summary>
        /// <param name="busId">The I2C bus ID where the device will be made available.</param>
        /// <param name="deviceAddress">The address of the I2C slave device.</param>
        /// <exception cref="ArgumentException">
        /// <para>
        /// Thrown when <paramref name="busId"/> is invalid.
        /// </para>
        /// <para>
        /// -- or --
        /// </para>
        /// <para>
        /// The specified <paramref name="busId"/> is being used either has slave or master device.
        /// </para>
        /// </exception>
        public I2cSlaveDevice(
            int busId,
            int deviceAddress)
        {
            _busId = busId;
            _deviceAddress = deviceAddress;

            // create the lock object
            _syncLock = new object();

            // create the buffer
            _buffer = new byte[1];
        }

        /// <summary>
        /// Reads a byte from the I2C master.
        /// </summary>
        /// <param name="value">The byte read from the I2C master.</param>
        /// <param name="timeout">The timeout in milliseconds to wait for the read to complete. Default is <see cref="Timeout.Infinite"/></param>
        /// <returns> The byte read from the I2C master.</returns>
        public bool ReadByte(
            out byte value,
            int timeout = Timeout.Infinite)
        {
            lock (_syncLock)
            {
                var buffer = new SpanByte(_buffer);

                var count = NativeTransmit(
                    buffer,
                    null,
                    timeout);

                value = buffer[0];

                return count == 1;
            }
        }

        /// <summary>
        /// Reads data from the I2C master.
        /// </summary>
        /// <param name="buffer">The buffer to read the data from the I2C master.</param>
        /// <param name="timeout">The timeout in milliseconds to wait for the read to complete. Default is <see cref="Timeout.Infinite"/>.</param>
        /// <returns>The number of bytes read from the I2C master.</returns>
        /// <remarks>
        /// <para>
        /// The length of <paramref name="buffer"/> determines how much data to read from the I2C master.
        /// </para>
        /// <para>
        /// The return value will be the same as the length of <paramref name="buffer"/> if the read was successful.
        /// </para>
        /// <para>
        /// No exception will be thrown if the read was not successful. The return value will indicate how many bytes were read.
        /// </para>
        /// </remarks>
        public int Read(
            SpanByte buffer,
            int timeout = Timeout.Infinite)
        {
            lock (_syncLock)
            {
                return NativeTransmit(
                    buffer,
                    null,
                    timeout);
            }
        }

        /// <summary>
        /// Writes a byte to the I2C master.
        /// </summary>
        /// <param name="value">The byte to be written to the I2C master.</param>
        /// <param name="timeout">The timeout in milliseconds to wait for the write to complete. Default is <see cref="Timeout.Infinite"/></param>
        /// <returns><see langword="true"/> if the byte was written successfully; otherwise, <see langword="false"/>.</returns>
        public bool WriteByte(
            byte value,
            int timeout = Timeout.Infinite)
        {
            lock (_syncLock)
            {
                // copy value
                _buffer[0] = value;

                return NativeTransmit(
                    null,
                    new SpanByte(_buffer),
                    timeout) == 1;
            }
        }

        /// <summary>
        /// Writes data to the I2C master.
        /// </summary>
        /// <param name="buffer">The buffer to be written to the I2C master.</param>
        /// <param name="timeout">The timeout in milliseconds to wait for the write to complete. Default is <see cref="Timeout.Infinite"/></param>
        /// <returns> The number of bytes written to the I2C master.</returns>
        /// <remarks>
        /// <para>
        /// The length of <paramref name="buffer"/> determines how much data to write to the I2C master.
        /// </para>
        /// <para>
        /// The return value will be the same as the length of <paramref name="buffer"/> if the write was successful.
        /// </para>
        /// <para>
        /// No exception will be thrown if the write was not successful. The return value will indicate how many bytes were written.
        /// </para>
        /// </remarks>
        public int Write(
            SpanByte buffer,
            int timeout = Timeout.Infinite)
        {
            lock (_syncLock)
            {
                return NativeTransmit(
                    null,
                    buffer,
                    timeout);
            }
        }

        #region IDisposable Support

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                NativeDispose();

                _disposed = true;
            }
        }

        /// <inheritdoc/>
        ~I2cSlaveDevice()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region external calls to native implementations

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeInit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeDispose();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeTransmit(SpanByte readBuffer, SpanByte writeBuffer, int timeout);

        #endregion
    }
}
