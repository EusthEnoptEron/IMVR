// Init source stream (Codec -> PCM)
var m_source = CodecFactory.Instance.GetCodec(filename);

// Determine the bit depth and choose an appropriate decoder
// (PCM (byte) -> Samples (float))
switch (m_source.WaveFormat.BitsPerSample)
{
    case 8:
        m_decoder = new Pcm8BitToSample(m_source);
        break;
    case 16:
        m_decoder = new Pcm16BitToSample(m_source);
        break;
    case 24:
        m_decoder = new Pcm24BitToSample(m_source);
        break;
    default:
        Debug.LogError("No converter found!");
        return;
}

// Create a clip with the decoder stream as the input
Clip = AudioClip.Create(m_name,
    (int)(m_decoder.Length / m_decoder.WaveFormat.Channels),
    m_decoder.WaveFormat.Channels,
    m_decoder.WaveFormat.SampleRate,
    true,
    OnReadAudio,
    OnSetPosition);