using System.Speech.Synthesis;

namespace Wanderer.Software.Speech
{
    public class SpeechSynthesisServer : Module
    {
        private SpeechSynthesizer speechSynthesizer;
        public static SpeechSynthesisServer Instance { get; } = new SpeechSynthesisServer();
        protected SpeechSynthesisServer()
        {
            State = ModuleStateEnu.Created;
            Name = $"{GetType().Name}";
            speechSynthesizer = new SpeechSynthesizer();
            State = ModuleStateEnu.Started;
        }

        public void Speak(string v)
        {
            speechSynthesizer.SpeakAsync(v);
        }

        public void SpeakIfNotSpeaking(string v)
        {
            if (speechSynthesizer.State != SynthesizerState.Speaking)
            {
                speechSynthesizer.SpeakAsync(v);
            }
        }
    }
}