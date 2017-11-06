using UnityEngine;
using System.Collections.Generic;

namespace OceanSim
{
    /// <summary>
    /// Old class providing ocean buoyancy.
    /// </summary>
	public class Boyancy : MonoBehaviour 
	{
		#region Private Variables and Properties
		private Ocean ocean;
		#endregion
		
		#region Public Variables and Properties
		public bool underWater = false;
		public bool prevUnderWater = false;
		#endregion
		
		#region System Methods
		/*void Start () 
		{
			RenderCheck();
			SetPhysics();
			
			//ocean = Ocean.Singleton;
			
			//Invisible Blobs That Hold Up Objects
			blobs = new List<Vector3> ();
			
			//List of previous boyancies
			prevBoya = new List<float[]>();
			
			StartSinkForces();
		}
		
		void Update()
		{
			if (ocean != null) 
			{

				visible = _renderer.isVisible;

				//HeightCorrection();
				//put object on the correct height of the sea surface when it has visibilty checks on and it became visible again.
				if(visible != lastvisible) {
					if(visible && !lastvisible) {
						if(Time.frameCount-lastFrame>15) {
							float off = ocean.GetChoppyAtLocation(transform.position.x, transform.position.z);
							float y = ocean.GetWaterHeightAtLocation2 (transform.position.x-off, transform.position.z);
							transform.position = new Vector3(transform.position.x, y, transform.position.z);
							lastFrame = Time.frameCount;
						}
					}
					lastvisible = visible;
				}

				//GravityUsage();
				//prevent use of gravity when buoyancy is disabled
				if(cvisible) {
					if(useGravity) {
						if(!visible) {
								rrigidbody.useGravity=false;
								if(wvisible && svisible) return;
						} else {
								rrigidbody.useGravity = true;
							}
					}else {
						if(!visible) { if(wvisible && svisible) return;} 
					}
				}

				float coef = dampCoeff;
				int index = 0, k=0;

				int ran = (int)Random.Range(0, blobs.Count-1);


				for(int j = 0; j<blobs.Count; j++) {

					wpos = transform.TransformPoint (blobs[j]);
					//get a random blob to apply a force with the choppy waves
					if(ChoppynessAffectsPosition) { if(j == ran)  cpos = wpos; }

					if(!cvisible || visible) {
						float buyancy = magnitude * (wpos.y);

						if (ocean.enabled) {
							if(ocean.canCheckBuoyancyNow[0]==1) {
								float off = 0;
									if(ocean.choppy_scale>0) off = ocean.GetChoppyAtLocation(wpos.x, wpos.z);
								if(moreAccurate) {	
									buyancy = magnitude * (wpos.y - ocean.GetWaterHeightAtLocation2 (wpos.x-off, wpos.z));
								}else {
									buyancy = magnitude * (wpos.y - ocean.GetWaterHeightAtLocation (wpos.x-off, wpos.z));
									buyancy = Lerp(prevBuoyancy, buyancy, 0.5f);
									prevBuoyancy = buyancy;
								}
								bbboyancy = buyancy;
							} else {
								buyancy = bbboyancy;
							}
						}

						if (sink) { buyancy = System.Math.Max(buyancy, -3) + sinkForces[index++]; }

						float damp = rrigidbody.GetPointVelocity (wpos).y;

						float bbuyancy = buyancy;

						//interpolate last (int interpolation) frames to smooth out the jerkiness
						//interpolation will be used only if the renderer is visible
						if(interpolate) {
							if(visible) {
								prevBoya[k][tick] = buyancy;
								bbuyancy=0;
								for(int i=0; i<intplt; i++) { bbuyancy += prevBoya[k][i]; }
								bbuyancy *= iF;
							}
						}
						rrigidbody.AddForceAtPosition (-Vector3.up * (bbuyancy + coef * damp), wpos);
						k++;
					}
				}

				if(interpolate) { tick++; if(tick==intplt) tick=0; }

				tack++; if (tack == (int)Random.Range(2, 9) ) tack=0;
				if(tack>9) tack =1;

				//if the boat has high speed do not influence it (choppyness and wind)
				//if it has lower then fact then influence it depending on the speed .
				float fact = rrigidbody.velocity.magnitude * 0.02f;

				//this code is quick and dirty
				if(fact<1) {
					float fact2 = 1-fact;
					//if the object gets its position affected by the force of the choppy waves. Useful for smaller objects).
					if(ChoppynessAffectsPosition) {
						if(!cvisible || visible) {
							if(ocean.choppy_scale>0) {
								if(moreAccurate) {
									if(tack==0) rrigidbody.AddForceAtPosition (-Vector3.left * (ocean.GetChoppyAtLocation2Fast() * ChoppynessFactor*Random.Range(0.5f,1.3f))*fact2, cpos);
									else rrigidbody.AddForceAtPosition (-Vector3.left * (ocean.GetChoppyAtLocation2Fast() * ChoppynessFactor*Random.Range(0.5f,1.3f))*fact2, transform.position);
								} else {
									if(tack==0) rrigidbody.AddForceAtPosition (-Vector3.left * (ocean.GetChoppyAtLocationFast() * ChoppynessFactor*Random.Range(0.5f,1.3f))*fact2, cpos);
									else rrigidbody.AddForceAtPosition (-Vector3.left * (ocean.GetChoppyAtLocationFast() * ChoppynessFactor*Random.Range(0.5f,1.3f))*fact2, transform.position);
								}
							}
						}
					}
					//if the object gets its position affected by the wind. Useful for smaller objects).
					if(WindAffectsPosition) {
						if(!wvisible || visible) {
							if(tack==1) rrigidbody.AddForceAtPosition(new Vector3(ocean.pWindx, 0 , ocean.pWindy) * WindFactor*fact2, cpos);
							else rrigidbody.AddForceAtPosition(new Vector3(ocean.pWindx, 0 , ocean.pWindy) * WindFactor*fact2, transform.position);
						}
					}
				}

				//the object will slide down a steep wave
				//modify it to your own needs since it is a quick and dirty method.
				if(xAngleAddsSliding) {
					if(!svisible || visible) {
						float xangle = transform.localRotation.eulerAngles.x;
						currAngleX = (int)xangle;

						if(prevAngleX != currAngleX) {
							
							float fangle=0f;

							if(xangle>270 && xangle<355) {
								fangle = (360-xangle)*0.1f;
								accel -= fangle* slideFactor; if(accel<-20) accel=-20;
								}

							if(xangle>5 && xangle<90) {
								fangle = xangle*0.1f;
								accel += fangle* slideFactor;  if(accel>20) accel=20;
							}

							prevAngleX = currAngleX;
						}
					
						if((int)accel!=0) rrigidbody.AddRelativeForce (Vector3.forward * accel, ForceMode.Acceleration);
						if(accel>0) { accel-= 0.05f;	if(accel<0) accel=0; }
						if(accel<0) { accel+= 0.05f; if(accel>0) accel=0; }
					}
				}

				underWater = HeightInWater.underwater;
				if (underWater != prevUnderWater)
				{
					ResetBuoyancy();
				}

			}
		}*/
		#endregion
		
		#region Custom Methods
		/*Brief: Runs at beginning to check if the renderer  components are valid
		* 
		*/
		/*void RenderCheck()
		{
			if(!_renderer) 
			{
				_renderer = GetComponent<Renderer>();
				if(!_renderer) {
					_renderer = GetComponentInChildren<Renderer>();
				}
			}

			if(_renderer && renderQueue>0)
			{
				_renderer.material.renderQueue = renderQueue;
			}
			
			if(!_renderer) {
				if(cvisible) { Debug.Log("Renderer to check visibility not assigned."); cvisible = false; }
				if(wvisible) { Debug.Log("Renderer to check visibility not assigned."); wvisible = false; }
				if(svisible) { Debug.Log("Renderer to check visibility not assigned."); svisible = false; }
			}
		}*/
		
		/*Brief: Adds sink forces to object on water
		* 
		*/
		/*void StartSinkForces()
		{
			Vector3 bounds = GetComponent<BoxCollider> ().size;
			
			//render alyers and slices
			if(SlicesX<2) SlicesX=2;
			if(SlicesZ<2) SlicesZ=2;
			
			float length = bounds.z;
			float width = bounds.x;
			
			float xstep = 1.0f / ((float)SlicesX - 1f);
			float ystep = 1.0f / ((float)SlicesZ - 1f);
			
			sinkForces = new List<float>();
			float totalSink = 0;
			for (int x=0; x<SlicesX; x++) {
				for (int y=0; y<SlicesX; y++) {

					 blobs.Add(new Vector3((-0.5f + x * xstep) * width, -0.5f, (-0.5f + y * ystep) * length) + Vector3.up * ypos); //on top of water

					if(interpolate) { prevBoya.Add(new float[interpolation]); }
					
					float force =  Random.Range(0f,1f);
					force = force * force;
					totalSink += force;
					sinkForces.Add(force);
					i++;
				}		
			}
			
			// normalize the sink forces
			for (int j=0; j< sinkForces.Count; j++)	{
				sinkForces[j] = sinkForces[j] / totalSink * sinkForce;
			}
		}
		
		//Activates physics and rigid body characteristics
		void SetPhysics()
		{
			if(dampCoeff<0) dampCoeff = Mathf.Abs(dampCoeff);
			rrigidbody =  GetComponent<Rigidbody>();
			useGravity = rrigidbody.useGravity;
			if(interpolation>0) {
				interpolate = true;
				iF = 1/(float)interpolation;
				intplt = interpolation;
			}
			rrigidbody.centerOfMass = new Vector3 (0.0f, CenterOfMassOffset, 0.0f);
		}*/
		#endregion		
	}
}