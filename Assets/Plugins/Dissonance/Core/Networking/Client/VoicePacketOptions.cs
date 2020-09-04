namespace Dissonance.Networking.Client
{
    internal struct VoicePacketOptions
    {
        public int ChannelSessionRange
        {
            // Detect range of `ChannelSession` field based on the upper flag in the bitfield.
            // Old peers don't set this field so we know the range is 2 bits.
            // New peers always set this field so we know the range is 7 bits.
            get { return (_bitfield & 0x80) == 0 ? 4 : 128; }
        }

        public byte ChannelSession
        {
            get { return (byte)(_bitfield & 0x7F); }
        }

        private readonly byte _bitfield;
        public byte Bitfield { get { return _bitfield; } }

        private VoicePacketOptions(byte bitfield)
        {
            _bitfield = bitfield;
        }

        public static VoicePacketOptions Unpack(byte bitfield)
        {
            return new VoicePacketOptions(
                bitfield
            );
        }

        public static VoicePacketOptions Pack(byte channelSession)
        {
            // There are two formats for the following byte:
            // - Old clients do not set the first 6 bits and the last 2 bits are used as a 2 bit session number.
            // - New clients set the MSB to 1 (to indicate that the new format is in use) and use the remaining 7 bits as a session number.
            // These two formats are compatible. Newer clients know to look at the top bit and use the extended range only if it's available.
            // Older clients just interpret the bottom two bits of the 7 bit number and ignore the rest (which is equivalent to just sending a 2 bit number).
            var bitfield = (byte)(
                1 << 8 |
                (channelSession % 128) 
            );

            return new VoicePacketOptions(
                bitfield
            );
        }
    }
}
