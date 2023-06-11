using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    private float distanceX;
    private float distanceY;
    private float spread = 20;
    // Max size will make a map with a Square of double this size
    public static float maxSize = 50;
    public List<GameObject> rooms;
    private List<Vector3> takenPos = new List<Vector3>();
    public GameObject endRoom;
    public List<GameObject> Lrooms;
    private Vector3 genPos;
    private bool genFail;
    private float LroomNum;
    private GameObject roomGen;
    public List<GameObject> Entities;
    public Transform Player_;
    public GameObject Walter;
    public List<quandaleBrain> Quandles;
    public playerController controller;
    private int EntSpawnMin = 500;
    private int EntSpawnMax = 800;
    private List<Transform> roomlist;
    private List<DisableLightifPlayerClose> roomLights = new List<DisableLightifPlayerClose>();
    int roomUpdateProgress = 0;

    // Start is called before the first frame update
    void Start()
    {
        roomlist = new List<Transform>();
        //generate entities

        foreach (GameObject E in Entities)
        {
            int x = Random.Range(-EntSpawnMax, EntSpawnMax);
            int z = Random.Range(-EntSpawnMax, EntSpawnMax);
            if (x >= -EntSpawnMin && x <= EntSpawnMin && z >= -EntSpawnMin && z <= EntSpawnMin)
            {
                if (x >= 0)
                {
                    x = EntSpawnMin;
                }
                else if (x < 0)
                {
                    x = -EntSpawnMin;
                }
            }
            float y = E.transform.position.y;
            GameObject E_ = Instantiate(E, new Vector3(x, y, z), Quaternion.identity);
            E_.GetComponent<Grid>().player = Player_.transform;
            E_.GetComponent<Pathfinder>().player = Player_.transform;
            if (E_.GetComponent<quandaleBrain>() != null)
            {
                Quandles.Add(E_.GetComponent<quandaleBrain>());
            }
            //give the player a reference to all of em
            if (E_.GetComponent<shrekBrain>() != null)
            {
                controller.Shrek = E_.GetComponent<shrekBrain>();
            }
            if (E_.GetComponent<imposterBrain>() != null)
            {
                controller.Imposter = E_.GetComponent<imposterBrain>();
            }
        }
        controller.QuandaleA = Quandles[0];
        controller.QuandaleB = Quandles[1];
        controller.QuandaleC = Quandles[2];
        // generate 4 walters
        for (int i = 0; i <= 3; i++)
        {
            int x = Random.Range(-700, 700);
            int z = Random.Range(-700, 700);
            GameObject E = Instantiate(Walter, new Vector3(x, 0, z), Quaternion.identity);
            E.GetComponent<walterBrain>().player = Player_;
        }
        //link up all of the quandales
        Quandles[0].Quandale1 = Quandles[1];
        Quandles[0].Quandale2 = Quandles[2];
        Quandles[1].Quandale1 = Quandles[0];
        Quandles[1].Quandale2 = Quandles[2];
        Quandles[2].Quandale1 = Quandles[0];
        Quandles[2].Quandale2 = Quandles[1];

        //Generate the end room, then rotate it a random amount
        var endX = (int)Random.Range(1-maxSize, maxSize-4)*spread;
        var endY = (int)Random.Range(1-maxSize, maxSize-4)*spread;
        roomGen = Instantiate(endRoom, new Vector3(endX, 0, endY), transform.rotation, transform);
        roomGen.transform.RotateAround(new Vector3(endX+spread*(float)1.5, 0, endY+spread*(float)1.5), Vector3.down, 0);
        radarScript.target = roomGen.transform.position;

        //Make sure nothing else generates in the room
        for (int i = 0; i < 4; i = i + 1)
        {
            for (int iv = 0; iv < 4; iv = iv + 1)
            {
                takenPos.Add(new Vector3(endX, 0, endY));
                endY = endY + spread;
            }
            endX = endX + spread;
            endY = endY - spread*4;
        }
        //Generate some large rooms

        //determine amount of large rooms(4% of total)

        LroomNum = Mathf.Pow(maxSize * 2, 2) / 100;

        for (int i = 0; i < LroomNum; i++)
        {
            var X = (int)Random.Range(-maxSize, maxSize-1) * spread;
            var Y = (int)Random.Range(-maxSize, maxSize-1) * spread;
            //Generate a position
            genPos = new Vector3(X, 0, Y);
            //Check if the current position is taken
            for (int ii = 0; ii < takenPos.Count; ii = ii + 1)
            {
                if (takenPos[ii] == genPos)
                {
                    genFail = true;
                    ii = takenPos.Count;
                }
            }
            // generate a room, rotate it, and ensure nothing else generates there if nothing is there, otherwise try again
            if (!genFail)
            {
                roomGen = Instantiate(Lrooms[Random.Range(0, Lrooms.Count)], genPos, transform.rotation, transform);
                roomLights.Add(roomGen.GetComponent<DisableLightifPlayerClose>().Init(Player_));
                roomGen.transform.RotateAround(new Vector3(X + spread * (float)0.5, 0, Y + spread * (float)0.5), Vector3.down, 0);

                //it isn't worth it to put 2 for loops for this I think
                takenPos.Add(new Vector3(X, 0, Y));
                takenPos.Add(new Vector3(X + spread, 0, Y + spread));
                takenPos.Add(new Vector3(X + spread, 0, Y));
                takenPos.Add(new Vector3(X, 0, Y + spread));
                roomlist.Add(roomGen.transform);

            } else
            {
                i--;
            }
            //reset Fail check, move on to next room
            genFail = false;
        }
        //Set some variables
        distanceX = -maxSize * spread;
        //Generate small rooms
        while (distanceX < maxSize * spread)
        {
            //Reset the Y
            distanceY = -maxSize * spread;
            //Generate one row
            while (distanceY < maxSize * spread)
                {
                //Generate a position
                genPos = new Vector3(distanceX, 0, distanceY);
                //Check if the current position is taken
                for (int ii = 0; ii < takenPos.Count; ii = ii + 1)
                {
                    if (takenPos[ii] == genPos)
                    {
                        genFail = true;
                        ii = takenPos.Count;
                    }
                }
                // generate a room if nothing is there, otherwise move on
                if (!genFail)
                {
                    roomGen = Instantiate(rooms[Random.Range(0, rooms.Count)], genPos, Quaternion.Euler(new Vector3(0, 90 * Random.Range(0, 3), 0)), transform);
                    roomLights.Add(roomGen.GetComponent<DisableLightifPlayerClose>().Init(Player_));
                    roomlist.Add(roomGen.transform);
                }
                //reset Fail check, move on to next row
                genFail = false;
                distanceY = distanceY + spread;
            }
            //Make X higher to generate the next row
            distanceX = distanceX + spread;
        }
        //InvokeRepeating("UpdateRooms", 0, 5); this creates instability with frame rate
    }
    void Update()//function that updates all of the rooms, disabling them if the player is too far away
    {//this one is also a bit slow (could avoid the nessecity to iterate over all the rooms by arranging roomlist as a 2d array of sorts)
        float dst = 0;
        Vector3 position = Player_.position;
        int i = 0;
        for (; i < 100; i++)
        {
            Transform currentRoom = roomlist[roomUpdateProgress];
            dst = (position.x - currentRoom.position.x)* (position.x - currentRoom.position.x) + (position.z - currentRoom.position.z)* (position.z - currentRoom.position.z);
            if (dst > 40000)
            {
                currentRoom.gameObject.SetActive(false);
            } else
            {
                currentRoom.gameObject.SetActive(true);
                roomLights[roomUpdateProgress].updateLight();
            }
            roomUpdateProgress++;
            if (roomUpdateProgress >= roomlist.Count) { roomUpdateProgress = 0; }
        }
    }
}
