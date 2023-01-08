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


    // Start is called before the first frame update
    void Start()
    {
        
        //Generate the end room, then rotate it a random amount
        var endX = (int)Random.Range(1-maxSize, maxSize-4)*spread;
        var endY = (int)Random.Range(1-maxSize, maxSize-4)*spread;
        roomGen = Instantiate(endRoom, new Vector3(endX, 0, endY), transform.rotation);
        roomGen.transform.RotateAround(new Vector3(endX+spread*(float)1.5, 0, endY+spread*(float)1.5), Vector3.down, 0);
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
                roomGen = Instantiate(Lrooms[Random.Range(0, Lrooms.Count)], genPos, transform.rotation);
                roomGen.transform.RotateAround(new Vector3(X + spread * (float)0.5, 0, Y + spread * (float)0.5), Vector3.down, 0);

                //it isn't worth it to put 2 for loops for this I think
                takenPos.Add(new Vector3(X, 0, Y));
                takenPos.Add(new Vector3(X + spread, 0, Y + spread));
                takenPos.Add(new Vector3(X + spread, 0, Y));
                takenPos.Add(new Vector3(X, 0, Y + spread));

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
                    Instantiate(rooms[Random.Range(0, rooms.Count)], genPos, Quaternion.Euler(new Vector3(0, 90 * Random.Range(0, 3), 0)));
                }
                //reset Fail check, move on to next row
                genFail = false;
                distanceY = distanceY + spread;
            }
            //Make X higher to generate the next row
            distanceX = distanceX + spread;
        }
    }
}
