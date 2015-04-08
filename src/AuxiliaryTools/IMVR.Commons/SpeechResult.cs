using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    /// <summary>
    /// Result of a speech recognition.
    /// </summary>
    public struct SpeechResult
    {
        /// <summary>
        /// Gets or sets the text that was recognized.
        /// </summary>
        public string Text;

        /// <summary>
        /// Gets or sets the confidence (0..1) of the result.
        /// </summary>
        public double Confidence;

        /// <summary>
        /// Gets or sets whether or not this result is final (as opposed to a preliminary guess)
        /// </summary>
        public bool Final;

        /// <summary>
        /// Gets or sets the ID of the recognition attempt.
        /// </summary>
        public int ID;


        public override string ToString()
        {
            return String.Format("{0}\t{1:0.00}\t{2}\t{3}", Text, Confidence, Final, ID);
        }


        public static SpeechResult Deserialize(string str)
        {
            var parts = str.Split('\t');
            if (parts.Length != 4) throw new Exception("Invalid format.");

            var result = new SpeechResult();
            result.Text = parts[0];
            result.Confidence = Convert.ToDouble(parts[1]);
            result.Final = Convert.ToBoolean(parts[2]);
            result.ID = Convert.ToInt32(parts[3]);

            return result;
        }
    }
}
