using System;
using System.Net.Sockets;
using System.Threading;

namespace Base.Network.Shared
{
    /// <summary>
    /// This class is responsible for represents the pool  manager of application.
    /// </summary>
    public static class PoolManager
    {
        /// <summary>
        /// The initialized value flag.
        /// </summary>
        [ThreadStatic]
        private static bool initialized;

        /// <summary>
        /// The buffer writer objects pool instance..
        /// </summary>
        [ThreadStatic]
        private static Pool<BufferWriter> _BufferWriterPool;

        /// <summary>
        /// The buffer reader objects pool instance..
        /// </summary>
        [ThreadStatic]
        private static Pool<BufferReader> _BufferReaderPool;

        /// <summary>
        /// The socket async event args objects pool instance..
        /// </summary>
        [ThreadStatic]
        private static Pool<SocketAsyncEventArgs> _socketAsyncEventArgsPool;

        /// <summary>
        /// The initialize object lock.
        /// </summary>
        private static readonly object initializeLock = new object();

        /// <summary>
        /// This method is responsible for initialize the thread instance of objects.
        /// </summary>
        private static void ThreadInitialize()
        {
            var obj = initializeLock;

            Monitor.Enter(obj);

            try
            {
                _BufferWriterPool = new Pool<BufferWriter>(2, () => new BufferWriter());
                _BufferReaderPool = new Pool<BufferReader>(2, () => new BufferReader());
                _socketAsyncEventArgsPool = new Pool<SocketAsyncEventArgs>(32, () => new SocketAsyncEventArgs());
            }
            finally
            {
                Monitor.Exit(obj);
            }

            initialized = true;
        }

        /// <summary>
        /// The buffer writer object instance.
        /// </summary>
        public static BufferWriter BufferWriter
        {
            get
            {
                if (!initialized)
                    ThreadInitialize();

                return _BufferWriterPool.GetInstance();
            }
        }
        
        /// <summary>
        /// The buffer reader object instance.
        /// </summary>
        public static BufferReader bufferReader
        {
            get
            {
                if (!initialized)
                    ThreadInitialize();

                return _BufferReaderPool.GetInstance();
            }
        }

        /// <summary>
        /// The socket async event args object instance.
        /// </summary>
        public static SocketAsyncEventArgs SocketAsyncEventArgs
        {
            get
            {
                if (!initialized)
                    ThreadInitialize();

                return _socketAsyncEventArgsPool.GetInstance();
            }
        }

        /// <summary>
        /// This method is reponsible for dispose the buffer writer object instance.
        /// </summary>
        /// <param name="BufferWriter">The buffer writer object instance</param>
        public static void DisposeBufferWriter(BufferWriter BufferWriter)
        {
            if (!initialized)
                ThreadInitialize();

            _BufferWriterPool.ReturnInstance(BufferWriter);
        }

        /// <summary>
        /// This method is reponsible for dispose the reader writer object instance.
        /// </summary>
        /// <param name="bufferReader">The buffer reader object instance</param>
        public static void DisposebufferReader(BufferReader bufferReader)
        {
            if (!initialized)
                ThreadInitialize();

            _BufferReaderPool.ReturnInstance(bufferReader);
        }

        /// <summary>
        /// This method is reponsible for dispose the socket async events args object instance.
        /// </summary>
        /// <param name="socketAsyncEventArgs">The socket async events args object instance</param>
        public static void DisposeSocketAsyncEventArgs(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (!initialized)
                ThreadInitialize();

            _socketAsyncEventArgsPool.ReturnInstance(socketAsyncEventArgs);
        }
    }
}
