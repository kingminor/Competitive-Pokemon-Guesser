const main = document.querySelector("#main");
const createGameForm = document.querySelector("#create-game");
const joinGameForm = document.querySelector("#join-game");
createGameForm.querySelector("input[type=submit]").disabled = true;
joinGameForm.querySelector("input[type=submit]").disabled = true;
const gameSection = document.querySelector("#game-section");
gameSection.style.display = "none";
const startGameBtn = document.querySelector("#startGameBtn");
const guessingSection = document.querySelector("#guessing-section");
guessingSection.style.display = "none";
const pkmnImage = document.querySelector("#pkmn-image");
const guessInput = document.querySelector("#whos-that-pokemon");
const guessPokemonBtn = document.querySelector("#guessPokemonBtn");
let playerID = null;
let gameID = null;

// Next several methods create the various methods required to recive the information
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5070/GameHub")
    .withAutomaticReconnect()
    .build();

connection.on("Connected", (id) => {
    playerID = id;
    console.log(`Recived playerID: ${playerID}`);
});

connection.on("Error", (err) => {
    console.log(`Error: ${err}`);
});

connection.on("Nickname Changed", () => {
    console.log("Nickname Changed");
});

connection.on("Game Created", (id) => {
    gameID = id;
    console.log(`Game Created with ID: ${gameID}`);
    ShowGameLobby();
});

connection.on("Joined Game", () => {
    console.log("Joined Game");
    ShowGameLobby();
});

connection.on("Game Started", (imageURL) => {
    console.log(`Game Started with image: ${imageURL}`);
    pkmnImage.src = imageURL;
    guessingSection.style.display = "flex";
});

connection.on("End Game", (reason, winnerPlayerID, pokemonName) => {
    if(reason === "Correct Guess"){
        console.log(`Player ${winnerPlayerID} guessed correctly! Pokemon was: ${pokemonName}`);
    }
    else if (reason === "Time Up"){
        console.log(pokemonName);
        console.log(`Time is up! The Pokemon was ${pokemonName}`);
    }
});

connection.on("Incorrect Guess", () => {
    console.log("Incorrect Guess");
});

connection.start()
    .then(() => {
        console.log("Connected");
        createGameForm.querySelector("input[type=submit]").disabled = false;
        joinGameForm.querySelector("input[type=submit]").disabled = false;
    })
    .catch(err => console.log(err));

// Event Listeners for the various forms
createGameForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const nickname = createGameForm.nickname.value;
    await connection.invoke("EditNickName", playerID, nickname);

    const maxPlayers = createGameForm.maxPlayers.value;
    const timeBeforeReveal = createGameForm.timeBeforeReveal.value;

    await connection.invoke("CreateGame", parseInt(maxPlayers), parseInt(timeBeforeReveal), playerID);
});

joinGameForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const nickname = joinGameForm.nickname.value;
    await connection.invoke("EditNickName", playerID, nickname);

    const gameToJoinID = joinGameForm.gameID.value;
    await connection.invoke("JoinGame", playerID, gameToJoinID);
});

function ShowGameLobby() {
    createGameForm.style.display = "none";
    joinGameForm.style.display = "none";
    gameSection.style.display = "flex";
    gameSection.querySelector("h1").innerText = `Join Code: ${gameID}`;
}

startGameBtn.addEventListener("click", async (event) => {
    event.preventDefault();
    await connection.invoke("StartGame", playerID);
});

guessPokemonBtn.addEventListener("click", async (event) => {
    event.preventDefault();
    let value = guessInput.value.trim();

    await connection.invoke("GuessPokemon", playerID, value);
});