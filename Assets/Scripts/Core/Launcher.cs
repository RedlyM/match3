using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace MatchThree.Core
{
    public class Launcher : IInitializable
    {
        public void Initialize()
        {
            SceneManager.LoadScene(1);
        }
    }
}