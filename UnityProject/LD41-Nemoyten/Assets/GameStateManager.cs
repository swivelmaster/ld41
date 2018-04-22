using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
	
	public enum EnemyType {
		DogO, ShootyChute, BullionCube, SpicyMeme, SteamedArtichoke
	}

	public struct Recipe {
		public int DogO;
		public int ShootyChute;
		public int BullionCube;
		public int SpicyMeme;
		public int SteamedArtichoke;

		public Recipe(int dogo, int shooty, int cube, int spicy, int steamed){
			DogO = dogo;
			ShootyChute = shooty;
			BullionCube = cube;
			SpicyMeme = spicy;
			SteamedArtichoke = steamed;
		}

		public bool Complete(){
			return DogO + ShootyChute + BullionCube + SpicyMeme + SteamedArtichoke == 0;
		}

		public bool IsValid(){
			// In other words, if any of these go negative, you lose.
			return DogO >= 0 && ShootyChute >= 0 && BullionCube >= 0 && SpicyMeme >= 0 && SteamedArtichoke >= 0;
		}
	}

	public static GameStateManager instance;

	public GameObject Player;

	public Recipe CurrentRecipe;

	void Start () {
		instance = this;
		CurrentRecipe = new Recipe(1, 0, 0, 0, 0);
	}
	
	public void UpdateRecipe(EnemyType enemyType){
		switch (enemyType){
			case EnemyType.BullionCube:
				CurrentRecipe.BullionCube--;
				break;
			case EnemyType.DogO:
				CurrentRecipe.DogO--;
				break;
			case EnemyType.ShootyChute:
				CurrentRecipe.ShootyChute--;
				break;
			case EnemyType.SpicyMeme:
				CurrentRecipe.SpicyMeme--;
				break;
			case EnemyType.SteamedArtichoke:
				CurrentRecipe.SteamedArtichoke--;
				break;
		}

		if (!CurrentRecipe.IsValid()){
			Debug.Log("TRIGGER LOSE CONDITION!");
		}
	}
	

	// void Update () {
		
	// }
}


