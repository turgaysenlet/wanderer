using System.Speech.Synthesis;

namespace Wanderer.Software.Speech
{
    public class SpeechSynthesisServerCls : ModuleCls
    {
        private SpeechSynthesizer speechSynthesizer;
        public static SpeechSynthesisServerCls Instance { get; } = new SpeechSynthesisServerCls();
        protected SpeechSynthesisServerCls()
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