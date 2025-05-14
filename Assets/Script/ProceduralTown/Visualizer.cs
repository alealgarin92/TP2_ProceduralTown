using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
    public class Visualizer : MonoBehaviour
    {
        public LSystemGenerator lSystem;
        List<Vector3> positions = new List<Vector3>();

        [SerializeField] RoadHelper roadHelper;
        [SerializeField] StructureHelper structureHelper;

        private int roadLenght = 8;
        private int lenght = 8;
        [SerializeField][Range(0f,90f)]private float angle = 90f;

        private bool waitingForTheRoad = false;
        [SerializeField] private GameObject playerPrefab;
        private GameObject playerInstance;
        [SerializeField] private Vector3 playerSpawnOffset = new Vector3(0, 1f, 0);

        public int Lenght
        {
            get
            {
               return lenght > 0? lenght : 1;
            }
             set => lenght = value;
        }
        public enum EncodingLetters
        {
            unknown = '1',
            save = '[',
            load = ']',
            draw = 'F',
            turnRight = '+',
            turnLeft = '-'
        }
        
        private void Start() 
        {
            roadHelper.finishedCoroutine += () => waitingForTheRoad = false;
            CreateTown();
        }

        public void CreateTown()
        {
            lenght = roadLenght;
            
            roadHelper.Reset();
            structureHelper.Reset();
         
            var sequence = lSystem.GenerateSentence();
            StartCoroutine(VisualizeSequence(sequence)); 
        }


        private IEnumerator VisualizeSequence(string sequence)
        {
            Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
            Vector3 currentPosition = Vector3.zero;

            Vector3 direction = Vector3.forward;
            Vector3 tempPosition = Vector3.zero;

         
            foreach(var letter in sequence)
            {
                if(waitingForTheRoad)
                {
                    yield return new WaitForEndOfFrame();
                }

                EncodingLetters encoding = (EncodingLetters)letter;

                switch(encoding)
                {
                    case EncodingLetters.unknown:
                        break;
                    case EncodingLetters.save:
                        savePoints.Push( new AgentParameters
                        {
                            position = currentPosition,
                            direction = direction,
                            lenght = Lenght
                        });
                        break;
                    case EncodingLetters.load:
                        if(savePoints.Count > 0)
                        {
                            var agentParameter = savePoints.Pop();
                            currentPosition = agentParameter.position;
                            direction = agentParameter.direction;
                            Lenght = agentParameter.lenght;
                        }
                        else
                        {
                            throw new System.Exception("Dont have saved point in Stack");
                        }
                        break;
                    case EncodingLetters.draw:
                        tempPosition = currentPosition;
                        currentPosition += direction * lenght;
                        StartCoroutine(roadHelper.PlaceStreetPositions(tempPosition, Vector3Int.RoundToInt(direction),lenght));
                        
                        waitingForTheRoad = true;
                        yield return new WaitForEndOfFrame();

                        Lenght -= 2;
                        break;
                    case EncodingLetters.turnRight:
                        direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                        break;
                    case EncodingLetters.turnLeft:
                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                        break;
                    default:
                        break;

                }
           
            }
            yield return new WaitForSeconds(0.1f);
            roadHelper.FixRoad();
            yield return new WaitForSeconds(0.8f);
            StartCoroutine(structureHelper.PlaceStructuresAroundRoad(roadHelper.GetRoadPositions()));

            SpawnPlayer(); //ubica el jugador
            PlaceExitPointFarFromPlayer(playerInstance.transform.position);
            // ubica la salida


        }
        private void SpawnPlayer() //ubicar el personaje en el mapa
        {
            List<Vector3Int> roadPositions = roadHelper.GetRoadPositions();
            if (roadPositions.Count == 0)
            {
                Debug.LogWarning("No hay calles para ubicar al jugador.");
                return;
            }

            Vector3Int spawnPoint = roadPositions[0]; // Usamos la primer calle generada
            Vector3 worldPosition = spawnPoint + new Vector3(0, 0.01f, 0); // Un poco elevado

            playerInstance = Instantiate(playerPrefab, worldPosition, Quaternion.identity);
        }

        [SerializeField] private GameObject exitPrefab;

        private void PlaceExitPointFarFromPlayer(Vector3 playerPosition)
        {
            List<Vector3Int> roadPositions = roadHelper.GetRoadPositions();
            Vector3Int farthestPoint = roadPositions[0];
            float maxDistance = 0f;

            foreach (var pos in roadPositions)
            {
                float distance = Vector3.Distance(pos, playerPosition);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = pos;
                }
            }

            // Rotación de la calle, usando la dirección calculada para la calle
            Vector3 direction = (farthestPoint - playerPosition).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            Vector3 spawnPosition = farthestPoint + new Vector3(0, 0f, 0); // 0f = sin altura extra
            Instantiate(exitPrefab, spawnPosition, rotation);
        }
    }
}