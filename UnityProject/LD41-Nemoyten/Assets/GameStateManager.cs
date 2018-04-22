using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour {
	
	public LayerMask EnemiesLayerMask;

	public enum EnemyType {
		DogO, ShootyShoot, BullionCube, SpicyMeme, SteamedArtichoke
	}

	public GameObject SpawnPointsContainer;
	List<Transform> SpawnPoints = new List<Transform>();

	public GameObject DogOPrefab;
	public GameObject ShootyShootPrefab;
	public GameObject BullionCubePrefab;
	public GameObject SteamedArtichokePrefab;

	public struct Recipe {
		public int DogO;
		public int ShootyShoot;
		public int BullionCube;
		public int SteamedArtichoke;

		public Recipe(int dogo, int shooty, int cube, int steamed){
			DogO = dogo;
			ShootyShoot = shooty;
			BullionCube = cube;
			SteamedArtichoke = steamed;
		}

		public bool Complete(){
			return DogO + ShootyShoot + BullionCube + SteamedArtichoke == 0;
		}

		public bool IsValid(){
			// In other words, if any of these go negative, you lose.
			return DogO >= 0 && ShootyShoot >= 0 && BullionCube >= 0 && SteamedArtichoke >= 0;
		}
	}

	public struct SpawnOdds {
		public float DogO;
		public float ShootyShoot;
		public float BullionCube;
		public float SteamedArtichoke;

		public SpawnOdds(float dogo, float shooty, float cube, float steamed){
			DogO = dogo;
			ShootyShoot = shooty;
			BullionCube = cube;
			SteamedArtichoke = steamed;
		}

		public EnemyType Roll(){
			float value = Random.value;
			if (value <= DogO) return EnemyType.DogO;
			if (value <= DogO + ShootyShoot) return EnemyType.ShootyShoot;
			if (value <= DogO + ShootyShoot + BullionCube) return EnemyType.BullionCube;
			return EnemyType.SteamedArtichoke;
		}
	}

	Queue<SpawnOdds> AllSpawnOdds = new Queue<SpawnOdds>();

	public static GameStateManager instance;

	public GameObject Player;

	public Recipe CurrentRecipe;
	public SpawnOdds CurrentSpawnOdds;

	public Text RecipeText;

	public Text RecipeCompleteText;

	void Start () {
		instance = this;

		AllRecipes.Enqueue(new Recipe(1, 0, 0, 0));
		AllRecipes.Enqueue(new Recipe(1, 1, 0, 0));
		AllRecipes.Enqueue(new Recipe(3, 2, 0, 0));
		AllRecipes.Enqueue(new Recipe(2, 0, 1, 0));
		AllRecipes.Enqueue(new Recipe(0, 1, 2, 0));
		AllRecipes.Enqueue(new Recipe(1, 2, 3, 0));
		AllRecipes.Enqueue(new Recipe(2, 2, 0, 1));
		AllRecipes.Enqueue(new Recipe(4, 3, 4, 1));
		AllRecipes.Enqueue(new Recipe(8, 4, 2, 2));
		AllRecipes.Enqueue(new Recipe(10, 5, 4, 2));

		AllSpawnOdds.Enqueue(new SpawnOdds(1f, 0f, 0f, 0f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.5f, .5f, 0f, 0f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.6f, .4f, 0f, 0f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.4f, .2f, .4f, 0f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.4f, .4f, .2f, 0f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.4f, .4f, .2f, 0f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.3f, .3f, .2f, .2f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.3f, .3f, .3f, .1f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.3f, .3f, .3f, .1f));
		AllSpawnOdds.Enqueue(new SpawnOdds(.3f, .3f, .3f, .1f));


		GetNextRecipe();
		UpdateRecipeText();

		for (int i=0;i<SpawnPointsContainer.transform.childCount-1;i++){
			SpawnPoints.Add(SpawnPointsContainer.transform.GetChild(i));
		}

		StartCoroutine(Spawner());
	}
	
	public void UpdateRecipe(EnemyType enemyType){
		switch (enemyType){
			case EnemyType.BullionCube:
				CurrentRecipe.BullionCube--;
				break;
			case EnemyType.DogO:
				CurrentRecipe.DogO--;
				break;
			case EnemyType.ShootyShoot:
				CurrentRecipe.ShootyShoot--;
				break;
			case EnemyType.SteamedArtichoke:
				CurrentRecipe.SteamedArtichoke--;
				break;
		}

		if (!CurrentRecipe.IsValid()){
			Player.GetComponent<FirstPersonController>().WrongRecipeDeath();
		} else if (CurrentRecipe.Complete()){
			if (AllRecipes.Count == 0){
				Player.GetComponent<FirstPersonController>().GameCompleteDeath();
				RecipeText.text = "REQUIRED INGREDIENTS:\nONE GAMER";
				return;
			} else {
				GetNextRecipe();
				RecipeCompleteText.text = "DELICIOUS!\nAND YET I STILL HUNGER...";
				StartCoroutine(ClearDeliciousText());
			}
		}

		UpdateRecipeText();
	}

	Queue<Recipe> AllRecipes = new Queue<Recipe>{};

	void GetNextRecipe(){
		CurrentRecipe = AllRecipes.Dequeue();
		CurrentSpawnOdds = AllSpawnOdds.Dequeue();
	}

	IEnumerator ClearDeliciousText(){
		yield return new WaitForSeconds(2f);
		RecipeCompleteText.text = "";
	}

	IEnumerator Spawner(){
		yield return new WaitForSeconds(3f);
		while (Player.GetComponent<FirstPersonController>().alive){
			AttemptToSpawn();
			yield return new WaitForSeconds(3f);
		}
	}

	void AttemptToSpawn(){
		EnemyType type = CurrentSpawnOdds.Roll();
		Debug.Log("Spawning enemy type " + type.ToString());
		
		int tries = 10;
		while (tries-- >= 0){
			int index = (int)Mathf.Floor(Random.Range(0, SpawnPoints.Count - 1));
			Transform spawnPoint = SpawnPoints[index];
			
			// Don't spawn enemies at this spawn location if there are already enemies nearby to prevent clustering
			// and overlapping.
			Collider[] enemies = Physics.OverlapSphere(spawnPoint.position, 4f, EnemiesLayerMask, QueryTriggerInteraction.Ignore);
			if (enemies.Length == 0){
				GameObject objectToInstantiate = DogOPrefab;
				
				switch (type){
					case EnemyType.ShootyShoot:
						objectToInstantiate = ShootyShootPrefab;
						break;
					case EnemyType.BullionCube:
						objectToInstantiate = BullionCubePrefab;
						break;
					case EnemyType.SteamedArtichoke:
						objectToInstantiate = SteamedArtichokePrefab;
						break;
				}

				Instantiate(objectToInstantiate, spawnPoint.position, objectToInstantiate.transform.rotation);
				Debug.Log("Succesfully instantiated enemy.");
				return;
			}
		}

		Debug.Log("Couldn't instantiate enemy for some reason.");
	}

	void UpdateRecipeText(){
		string text = "REQUIRED INGREDIENTS:\nDOG-O's: ";
		text += CurrentRecipe.DogO.ToString() + "\n";
		text += "SHOOTY-SHOOTS: " + CurrentRecipe.ShootyShoot.ToString() + "\n";
		text += "BULLION CUBES: " + CurrentRecipe.BullionCube.ToString() + "\n";
		text += "STEAMED ARTICHOKES: " + CurrentRecipe.SteamedArtichoke.ToString() + "\n";

		RecipeText.text = text;
	}
	

	// void Update () {
		
	// }
}


