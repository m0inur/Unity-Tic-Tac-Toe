using UnityEngine;
using UnityEngine.UI;

namespace Particle
{
    public class InitParticles : MonoBehaviour
    {
        public Image particle;

        private Image _particle;
        private Canvas _canvas;
        
        private float _particleW;
        private float _particleH;

        private float _randParticleSize;
        private float _particleCount;
        private float _canvasW;
        private float _canvasH;
        
        
        // Start is called before the first frame update
        private void Start()
        {
        _particleW = particle.GetComponent<RectTransform>().rect.width;
        _particleH = particle.GetComponent<RectTransform>().rect.height;
        
        _canvas = FindObjectOfType<Canvas>();
        _canvasW = _canvas.GetComponent<RectTransform>().rect.width;
        _canvasH = _canvas.GetComponent<RectTransform>().rect.height;
        
        _particleCount = 150;
        InitializeParticles();
        }
        
        #region Init particles

        private void InitializeParticles()
        {
            for (var i = 0; i < _particleCount; i++)
            {
                _particle = Instantiate(particle,
                    new Vector3(UnityEngine.Random.Range(_particleW, _canvasW),
                        -(UnityEngine.Random.Range(_particleH, _canvasH)), 0), Quaternion.identity);
                _particle.transform.SetParent(GameObject.Find("Particle Spawner").transform, false);

                // Randomize Opacity
                Image image = _particle.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b,
                    UnityEngine.Random.Range(0.2f, 0.6f));

                // Randomize particle sizes
                _randParticleSize = UnityEngine.Random.Range(_particleW, _particleW + 8);
                _particle.GetComponent<RectTransform>().sizeDelta = new Vector2(_randParticleSize, _randParticleSize);
            }
        }

        #endregion

    }
}
