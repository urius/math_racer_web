using UnityEngine;

namespace Infra.Controller
{
    public abstract class ControllerWithModelBase<TModel> : ControllerBase
        where TModel : class
    {
        protected TModel TargetModel { get; private set; }

        public void Mediate(Transform transform, TModel model)
        {
            TargetModel = model;

            Start(transform);
        }

        public override void Stop()
        {
            base.Stop();

            TargetModel = null;
        }
    }
}