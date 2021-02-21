using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 10f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] KeyCode leftTurnKey = KeyCode.D;
    [SerializeField] KeyCode rightTurnKey = KeyCode.A;
    [SerializeField] KeyCode thrustKey = KeyCode.Space;


    Rigidbody rigidbody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive){ return;  }
        switch (collision.gameObject.tag)
        {
            case "Friendly": print("Friendly");
                             break;
            case "Obstacle":
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(death);
                deathParticles.Play();
                Invoke("ReplayScene", 1f);
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(success);
                successParticles.Play();
                Invoke("LoadNextScene", 1f);
                break;
        }
    }

    private void ReplayScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScene);
    }

    private void LoadNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScene + 1);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(thrustKey))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void RespondToRotate()
    {
        rigidbody.freezeRotation = true;

        float rotationThisFrame = Time.deltaTime * rcsThrust;
        if (Input.GetKey(rightTurnKey))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(leftTurnKey))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidbody.freezeRotation = false;
    }
}
