using System;
using System.IO;
using System.Threading.Tasks;

namespace Extensions
{
    internal class StreamExtensions
    {
    }

    public class SubStream : Stream
    {
        private Stream UnderlyingStream { get; set; }
        private long StartingPosition { get; set; }
        private long EndingPosition { get; set; }

        public override bool CanRead => UnderlyingStream.CanRead;

        public override bool CanSeek => UnderlyingStream.CanSeek;

        public override bool CanWrite => UnderlyingStream.CanWrite;

        public override long Length => EndingPosition - StartingPosition;

        public override long Position
        {
            get
            {
                return UnderlyingStream.Position - StartingPosition;
            }
            set
            {
                var offset = value + StartingPosition;
                if (CheckOffsetBounds(offset))
                {
                    throw new IndexOutOfRangeException();
                }

                UnderlyingStream.Position = offset;
            }
        }

        public SubStream(Stream underlyingStream, long startingPosition, long endingPosition)
        {
            UnderlyingStream = underlyingStream;
            StartingPosition = startingPosition;
            EndingPosition = endingPosition;
        }

        public async Task CopyToAsync(Stream stream)
        {
            UnderlyingStream.Position = StartingPosition;
            UnderlyingStream.SetLength(StartingPosition + Length);
            await UnderlyingStream.CopyToAsync(stream);
        }

        public override void Flush()
        {
            UnderlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            offset += (int)StartingPosition;

            var lastPosition = offset + count;

            if (CheckOffsetBounds(offset, lastPosition))
            {
                throw new IndexOutOfRangeException();
            }

            return UnderlyingStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    offset += StartingPosition;
                    break;
                case SeekOrigin.Current:
                    offset += Position;
                    break;
                case SeekOrigin.End:
                    var extraEndingOffset = Length - EndingPosition;
                    offset += extraEndingOffset;
                    break;
                // Why is that even needed
                default:
                    throw new InvalidOperationException();
            }

            if (CheckOffsetBounds(offset))
            {
                throw new IndexOutOfRangeException();
            }

            return UnderlyingStream.Seek(offset, origin);
        }

        private bool CheckOffsetBounds(long rawOffset)
        {
            return CheckOffsetBounds(rawOffset, rawOffset);
        }

        private bool CheckOffsetBounds(long startingPos, long endingPos)
        {
            return startingPos < StartingPosition || endingPos > EndingPosition;
        }

        public override void SetLength(long value)
        {
            // One shouldn't do that i think
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            offset += (int)StartingPosition;

            var lastPosition = offset + count;

            if (CheckOffsetBounds(offset, lastPosition))
            {
                throw new IndexOutOfRangeException();
            }

            UnderlyingStream.Write(buffer, offset, count);
        }
    }
}
