// Determine the start and end index in the spectrum array
int m_startIndex = VisualizationHelper.Instance.FreqToIndex(startFrequency);
int m_endIndex = VisualizationHelper.Instance.FreqToIndex(endFrequency);

// Calculate root mean square over the frequency range
float rms = 0;
for(int i = m_startIndex; i < m_endIndex; i++) {
    rms += Mathf.Pow(VisualizationHelper.Instance.spectrum[m_startIndex], 2);
}
rms = Mathf.Sqrt( rms / (m_endIndex - m_startIndex) );