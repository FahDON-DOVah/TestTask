using System;
using System.IO;

namespace TestTask
{
    internal interface IReadOnlyStream
    {
        char ReadNextChar();
        void ResetPositionToStart();
        bool IsEof { get; }
    }

    public class ReadOnlyStream : IReadOnlyStream, IDisposable
    {
        private readonly StreamReader _reader;

        public ReadOnlyStream(string fileFullPath)
        {
            if (string.IsNullOrEmpty(fileFullPath))
                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(fileFullPath));

            if (!File.Exists(fileFullPath))
                throw new FileNotFoundException("Файл не найден.", fileFullPath);

            _reader = new StreamReader(fileFullPath);
        }

        public char ReadNextChar()
        {
            if (_reader.EndOfStream)
                throw new InvalidOperationException("Конец файла достигнут.");

            return (char)_reader.Read();
        }

        public void ResetPositionToStart()
        {
            if (_reader.BaseStream.CanSeek)
            {
                _reader.BaseStream.Seek(0, SeekOrigin.Begin);
                _reader.DiscardBufferedData();
            }
            else
            {
                throw new NotSupportedException("Поток не поддерживает перемотку.");
            }
        }

        public bool IsEof => _reader.EndOfStream;

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}
