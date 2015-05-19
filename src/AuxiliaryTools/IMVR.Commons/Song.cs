using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    /// <summary>
    /// Represents an indexed audio file.
    /// </summary>
    [ProtoContract(AsReferenceDefault = true)]
    public class Song : File
    {
        [ProtoMember(13)]
        public string Title { get; set; }

        [ProtoMember(1)]
        public Artist Artist { get; set; }

        [ProtoMember(2)]
        public Album Album { get; set; }

        [ProtoMember(3)]
        public uint TrackNo { get; set; }

        /// <summary>
        /// Gets or sets the duration in seconds.
        /// </summary>
        [ProtoMember(4)]
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets the beats-per-minute.
        /// </summary>
        [ProtoMember(5)]
        public float Tempo { get; set; }

        /// <summary>
        /// Gets or sets the liveness of this song.
        /// Detects the presence of an audience in the recording. The more confident that the track is live, the closer to 1.0 the attribute value. 
        /// Due to the relatively small population of live tracks in the overall domain, the threshold for detecting liveness is higher than for speechiness. 
        /// A value above 0.8 provides strong likelihood that the track is live. Values between 0.6 and 0.8 describe tracks that may or may not be live 
        /// or contain simulated audience sounds at the beginning or end. Values below 0.6 most likely represent studio recordings.
        /// </summary>
        [ProtoMember(6)]
        public float? Liveness { get; set; }

        /// <summary>
        /// Gets or sets the speechiness.
        /// Detects the presence of spoken words in a track. The more exclusively speech-like the recording (e.g. talk show, audio book, poetry), 
        /// the closer to 1.0 the attribute value. Values above 0.66 describe tracks that are probably made entirely of spoken words. 
        /// Values between 0.33 and 0.66 describe tracks that may contain both music and speech, either in sections or layered,
        /// including such cases as rap music. Values below 0.33 most likely represent music and other non-speech-like tracks.
        /// </summary>
        [ProtoMember(7)]
        public float? Speechiness { get; set; }

        /// <summary>
        /// Gets or sets the acousticness.
        /// Represents the likelihood a recording was created by solely acoustic means such as voice and acoustic instruments 
        /// as opposed to electronically such as with synthesized, amplified, or effected instruments. Tracks with low acousticness 
        /// include electric guitars, distortion, synthesizers, auto-tuned vocals, and drum machines, whereas songs with orchestral instruments, 
        /// acoustic guitars, unaltered voice, and natural drum kits will have acousticness values closer to 1.0.
        /// </summary>
        [ProtoMember(8)]
        public float? Acousticness { get; set; }


        /// <summary>
        /// Gets or sets the instrumentalness.
        /// </summary>
        [ProtoMember(9)]
        public float? Instrumentalness { get; set; }

        /// <summary>
        /// Gets or sets the danceability.
        /// Describes how suitable a track is for dancing using a number of musical elements (the more suitable for dancing, 
        /// the closer to 1.0 the value). The combination of musical elements that best characterize danceability include tempo, 
        /// rhythm stability, beat strength, and overall regularity.
        /// </summary>
        [ProtoMember(10)]
        public float? Danceability { get; set; }

        /// <summary>
        /// Gets or sets the energy of this song.
        /// Represents a perceptual measure of intensity and powerful activity released throughout the track. 
        /// Typical energetic tracks feel fast, loud, and noisy. For example, death metal has high energy, 
        /// while a Bach prelude scores low on the scale. Perceptual features contributing to this attribute 
        /// include dynamic range, perceived loudness, timbre, onset rate, and general entropy.
        /// </summary>
        [ProtoMember(11)]
        public float? Energy { get; set; }

        /// <summary>
        /// Gets or sets the variance of this song.
        /// </summary>
        [ProtoMember(12)]
        public float? Variance { get; set; }


        /// <summary>
        /// Gets or sets the valence.
        /// Describes the musical positiveness conveyed by a track. Tracks with high valence sound more positive (e.g., happy, cheerful, euphoric), 
        /// while tracks with low valence sound more negative (e.g. sad, depressed, angry). This attribute in combination with energy is a strong indicator 
        /// of acoustic mood, the general emotional qualities that may characterize the track's acoustics. Note that in the case of vocal music, 
        /// lyrics may differ semantically from the perceived acoustic mood.
        /// </summary>
        [ProtoMember(14)]
        public float? Valence { get; set; }

        /// <summary>
        /// Gets whether or not any meta data attributes have been filled in.
        /// </summary>
        public bool Annotated
        {
            get
            {
                return Liveness != null || Speechiness != null || Acousticness != null || Instrumentalness != null || Danceability != null || Energy != null || Variance != null || Valence != null;
            }
        }

        /// <summary>
        /// Gets whether or not all meta data attributes have been filled in.
        /// </summary>
        public bool FullyAnnotated
        {
            get
            {
                return Liveness != null && Speechiness != null && Acousticness != null && Instrumentalness != null && Danceability != null && Energy != null && Variance != null && Valence != null;
            }
        }
    }


}
