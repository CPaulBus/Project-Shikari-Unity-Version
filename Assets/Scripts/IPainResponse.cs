using UnityEngine;

namespace Assets.Scripts
{
    public interface IPainResponse
    {
        public void HandlePain(int Damage);

        public void HandleDeath();
    }
}