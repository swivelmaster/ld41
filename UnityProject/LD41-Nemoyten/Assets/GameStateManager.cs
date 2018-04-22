using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour {
	
	public enum EnemyType {
		DogO, ShootyShoot, BullionCube, SpicyMeme, SteamedArtichoke
	}

	public struct Recipe {
		public int DogO;
		public int ShootyShoot;
		public int BullionCube;
		public int SpicyMeme;
		public int SteamedArtichoke;

		public Recipe(int dogo, int shooty, int cube, int spicy, int steamed){
			DogO = dogo;
			ShootyShoot = shooty;
			BullionCube = cube;
			SpicyMeme = spicy;
			SteamedArtichoke = steamed;
		}

		public bool Complete(){
			return DogO + ShootyShoot + BullionCube + SpicyMeme + SteamedArtichoke == 0;
		}

		public bool IsValid(){
			// In other words, if any of these go negative, you lose.
			return DogO >= 0 && ShootyShoot >= 0 && BullionCube >= 0 && SpicyMeme >= 0 && SteamedArtichoke >= 0;
		}
	}

	public static GameStateManager instance;

	public GameObject Player;

	public Recipe CurrentRecipe;

	public Text RecipeText;

	public Text RecipeCompleteText;

	void Start () {
		instance = this;

		AllRecipes.Enqueue(new Recipe(1, 0, 0, 0, 0));
		AllRecipes.Enqueue(new Recipe(0, 1, 0, 0, 0));

		GetNextRecipe();
		UpdateRecipeText();
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
			case EnemyType.SpicyMeme:
				CurrentRecipe.SpicyMeme--;
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
	}

	IEnumerator ClearDeliciousText(){
		yield return new WaitForSeconds(2f);
		RecipeCompleteText.text = "";
	}

	void UpdateRecipeText(){
		string text = "REQUIRED INGREDIENTS:\nDOG-O's: ";
		text += CurrentRecipe.DogO.ToString() + "\n";
		text += "SHOOTY-SHOOTS: " + CurrentRecipe.ShootyShoot.ToString() + "\n";
		text += "BULLION CUBES: " + CurrentRecipe.BullionCube.ToString() + "\n";
		// Cutting spicy meme from game because no time to build a flying enemy
		// text += "SPICY MEMES: " + CurrentRecipe.SpicyMeme.ToString() + "\n";
		text += "STEAMED ARTICHOKES: " + CurrentRecipe.SteamedArtichoke.ToString() + "\n";

		RecipeText.text = text;
	}
	

	// void Update () {
		
	// }
}


