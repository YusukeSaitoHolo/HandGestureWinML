using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.AI.MachineLearning;
using Windows.Storage.Streams;
#endif

namespace HandGestureWinML
{
    public class HandGestureModel
    {
#if WINDOWS_UWP
        private LearningModel model;
        private LearningModelSession session;
        private LearningModelBinding binding;
        public static async Task<HandGestureModel> CreateFromStreamAsync(IRandomAccessStreamReference stream)
        {
            HandGestureModel learningModel = new HandGestureModel();
            learningModel.model = await LearningModel.LoadFromStreamAsync(stream);
            learningModel.session = new LearningModelSession(learningModel.model);
            learningModel.binding = new LearningModelBinding(learningModel.session);
            return learningModel;
        }
        public async Task<HandGestureOutput> EvaluateAsync(HandGestureInput input)
        {
            binding.Bind("input", input.Input120);
            var result = await session.EvaluateAsync(binding, "0");
            var output = new HandGestureOutput();
            output.label = result.Outputs["label"] as TensorString;
            return output;
        }
#endif
    }

    public sealed class HandGestureInput
    {
#if WINDOWS_UWP
        public TensorFloat Input120;
#endif
    }

    public sealed class HandGestureOutput
    {
#if WINDOWS_UWP
        public TensorString label;
        public IList<IDictionary<string, float>> probabilities;
#endif
    }



}

