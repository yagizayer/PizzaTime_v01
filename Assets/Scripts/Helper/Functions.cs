using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace Helper
{
    public class Functions : MonoBehaviour
    {
        /// <summary>
        /// Open a new tab at default browser with given url
        /// </summary>
        /// <param name="urlName">Desired web site</param>
        public void LoadURL(string urlName)
        {
            Application.OpenURL(urlName);
        }
        /// <summary>
        /// Open desired scene
        /// </summary>
        /// <param name="sceneName">Desired scene name</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        /// <summary>
        /// RefreshCurrentScene
        /// </summary>
        public void RefreshScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// Returns non-repetative random list from given object pool
        /// </summary>
        /// <param name="objectPool">The object list which contains all possible objects</param>
        /// <param name="desiredListCount">Length of list to be return.</param>
        /// <typeparam name="T">Any type which can be instantiated.</typeparam>
        /// <returns>List with given generic type</returns>
        public List<T> GetUniqueRandomList<T>(List<T> objectPool, int desiredListCount)
        {
            List<int> excludedIndexes = new List<int>();
            List<T> result = new List<T>();
            System.Random r = new System.Random();
            int retryCount = 0;
            while (true)
            {
                if (retryCount < 500) retryCount++;
                else
                {
                    Debug.LogWarning("Size error");
                    return new List<T>();
                }
                if (result.Count == desiredListCount) return result;

                int randomIndex = r.Next(0, objectPool.Count);
                if (excludedIndexes.Contains(randomIndex)) continue;

                result.Add(objectPool[randomIndex]);
                excludedIndexes.Add(randomIndex);
            }
        }
        public List<int> GetUniqueRandomIntList(int size)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < size; i++)
                result.Add(i);
            return GetUniqueRandomList<int>(result, size);
        }
    }
}
