using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TestMainScene
    {
        [UnityTest, Performance]
        public IEnumerator TestPerformanceLoadScene()
        {
            SceneManager.LoadScene("Scenes/MainScene");
            yield return null;
            
            yield return Measure.Frames()
                .WarmupCount(10)
                .MeasurementCount(200)
                .Run();
        }
        
        [UnityTest]
        public IEnumerator TestVerifyScene()
        {
            SceneManager.LoadScene("Scenes/MainScene");
            yield return null;
            
            var playerObject = GameObject.Find("Player #1");
            var gameManagerObject = GameObject.Find("GameManager");

            Assert.That(playerObject, Is.Not.Null);
            Assert.That(gameManagerObject, Is.Not.Null);
        }
    }
}
