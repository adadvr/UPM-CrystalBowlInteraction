using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.GlobalPosition
{
    public class GlobalPositioner : MonoBehaviour
    {
        float myRadiusPolar;
        float myThetaPolar;
        float myDegrees;

        float deltaR;
        float deltaTheta;

        float previousR;
        float previousTheta;

        public float[] rList = new float[5] { 0f, 0f, 0f, 0f, 0f };
        float[] thetaList = new float[5] { 0f, 0f, 0f, 0f, 0f };
        float[] absoluteRList = new float[5] { 0f, 0f, 0f, 0f, 0f };

        public float averageRDelta;
        public float averageThetaDelta;
        public float averageAbsoluteR;

        public float withinBounds;
        public float intensityCounter;

        public float controllerVibration;

        float intensityDecay;

        float timeCounter;
        float timeInterval;

        public bool collided;

        ParticleSystem particleSystem;


    // Start is called before the first frame update
        void Start()
        {

            deltaR = 0f;
            deltaTheta = 0f;
            timeInterval = 0.2f;

            timeCounter = Time.time + timeInterval;

            intensityCounter = 0;
            withinBounds = 0f;
            intensityDecay = 5f;

            collided = false;

            particleSystem = GetComponent<ParticleSystem>();

        }


    // Update is called once per frame
        void Update()
        {
            if (timeCounter < Time.time)
            {

                CalculateDeltas();
                CalculateWithinBounds();

                timeCounter = Time.time + timeInterval;

            }

            if (intensityCounter > 0)
            {
                intensityCounter -= intensityDecay;
            }

            if (intensityCounter <= 0)
            {
                intensityCounter = 0;
            }

            intensityCounter += withinBounds * (averageThetaDelta / 2f);

            intensityDecay = 1f + (intensityCounter / 50f);

            float tempVolume = intensityCounter / 100f;
            if (tempVolume > 1) { tempVolume = 1; }
            this.GetComponent<AudioSource>().volume = tempVolume;

            this.GetComponent<AudioSource>().pitch = 0.9f + (intensityCounter / 5000f);

            var emission = particleSystem.emission;
            emission.rateOverTime = intensityCounter;

            if (collided)
            {
            intensityCounter = 100f * Mathf.Abs(averageRDelta) ;
            collided = false;
            }

            controllerVibration = intensityCounter / 400f;

        // OVRInput.SetControllerVibration(controllerVibration, controllerVibration, OVRInput.Controller.RTouch);

        }

        private void OnCollisionEnter(Collision collision)
        {
            collided = true;
        }

        private void CalculateWithinBounds()
        {
            if (averageAbsoluteR > 4f && averageAbsoluteR < 6f)
            {
                withinBounds = 1 - Mathf.Abs(5 - averageAbsoluteR);
            }
            else
            {
                withinBounds = 0;
            }
        }

        private void CalculateDeltas()
        {
            
                deltaR = (myRadiusPolar - previousR) * 10f;
                deltaTheta = Mathf.Abs((myThetaPolar - previousTheta) * 10f);

                if (deltaTheta > 10f) { deltaTheta = myThetaPolar * 10f; }

                previousR = myRadiusPolar;
                previousTheta = myThetaPolar;
                

                for (int i = 1; i < rList.Length; i++)
                {
                    rList[i - 1] = rList[i];
                    thetaList[i - 1] = thetaList[i];
                    absoluteRList[i - 1] = absoluteRList[i];
                }

                rList[rList.Length - 1] = deltaR;
                thetaList[rList.Length - 1] = deltaTheta;
                absoluteRList[rList.Length - 1] = previousR * 10f;

                float sumaR = 0f;
                float sumaTheta = 0f;
                float sumaAbsoluteR = 0f;

                for (int i = 0; i < rList.Length; i++)
                {
                    sumaR += rList[i];
                    sumaTheta += Mathf.Abs(thetaList[i]);
                    sumaAbsoluteR += absoluteRList[i];
                }

                averageRDelta = sumaR / 5f;
                averageThetaDelta = sumaTheta / 5f;
                averageAbsoluteR = sumaAbsoluteR / 5f;

                myRadiusPolar = Mathf.Sqrt(Mathf.Pow(this.transform.position.x, 2f) + Mathf.Pow(this.transform.position.y, 2f));
                myThetaPolar = Mathf.Atan2(this.transform.position.y, this.transform.position.x);

                myDegrees = myThetaPolar * Mathf.Rad2Deg;
            

        
        }

        void FixedUpdate()
        {
        
        }
    }

}

