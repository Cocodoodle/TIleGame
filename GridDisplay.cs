using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDisplay : MonoBehaviour
{
    public Camera cam;

    public SpriteRenderer map;

    public Transform BuildingSelected;
    public int buildingCost;
    public Transform mainSettlement;

    public GameObject UIManager;

    private int RotationAmount = 0;

    public TextMeshProUGUI ModeText;

    public GridManager grid;
    public float detail = 1f;

    public Mode mode;
     
    public GameObject tile;
    public float presistancy, lan, scale;
    public int seed, octaves;
    public Vector2 offset;
    public Color[] colors;

    public bool randomizeSeed;

    void Start()
    {
        int sizeX = Mathf.RoundToInt(map.bounds.size.x);
        int sizeY = Mathf.RoundToInt(map.bounds.size.y);

        if (randomizeSeed)
        {
            seed = (int)Random.Range(-999999, 999999);
        }

        grid = new GridManager(sizeX, sizeY, detail, tile, seed, scale, octaves, presistancy, lan, offset, colors);
        Resources.AddOilResource(grid, 0.6f);
        Resources.AddGoldResource(grid, 0.5f);

        mode = Mode.Create;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotationAmount += 90;

            if(BuildingSelected.tag == "V_Road")
            {
                BuildingSelected.tag = "H_Road";
            }

            if(RotationAmount == 180)
            {
                RotationAmount = 0;

                if (BuildingSelected.tag == "H_Road")
                {
                    BuildingSelected.tag = "V_Road";
                }

            }
        }

        if (Input.GetMouseButtonDown(0) && UIManager.GetComponent<UIManager>().isGameStarted == false)
        {
            Vector3 pos = grid.GetWorldPos(Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).y));

            if (grid.GetCellXY(pos).celltype != "Deep Ocean" && grid.GetCellXY(pos).celltype != "Shallow Ocean")
            {
                Transform obj = Instantiate(mainSettlement, pos, Quaternion.identity);
                obj.tag = "Main";

                UIManager.GetComponent<UIManager>().BuildingMenuButton.SetActive(true);
                UIManager.GetComponent<UIManager>().isGameStarted = true;
                mode = Mode.Create;

                RotateWithAmount(obj.gameObject, RotationAmount);

                obj.localScale = obj.localScale * detail;

                Cell cellClicked = grid.GetCellXY(pos);
                cellClicked.OcccupingObject = obj.gameObject;
                cellClicked.cellOwned = true;

                foreach (Cell c in grid.GetCellNeighborList(cellClicked))
                {
                    c.cellOwned = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (mode)
            {
                case Mode.Create:
                    mode = Mode.Destroy;
                    ModeText.text = "Mode: Destroy";
                    break;
                case Mode.Destroy:
                    mode = Mode.Buy;
                    ModeText.text = "Mode: Buy";
                    break;
                case Mode.Buy:
                    mode = Mode.Create;
                    ModeText.text = "Mode: Create";
                    break;
            }
        }


        if (Input.GetMouseButtonDown(0) && mode == Mode.Create)
        {
            Vector3 pos = grid.GetWorldPos(Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).y));
            Debug.Log(grid.GetCellXY(pos).cellOwned);
            //Debug.Log(grid.GetCellXY(pos).goldAmount + " Gold");
            //Debug.Log(grid.GetCellXY(pos).oilAmount + " Barrels of Oil");

            if (grid.GetCellXY(pos).OcccupingObject == null && BuildingSelected != null && PlayerInfo.gold - buildingCost >= 0 && grid.GetCellXY(pos).cellOwned)
            {
                Transform obj = Instantiate(BuildingSelected, pos, Quaternion.identity);

                PlayerInfo.gold -= buildingCost;

                if(BuildingSelected.tag == "V_Road" || BuildingSelected.tag == "H_Road")
                {
                    obj.GetComponent<Road>().cell = grid.GetCellXY(pos);
                    obj.GetComponent<Road>().grid = grid;
                }

                RotateWithAmount(obj.gameObject, RotationAmount);

                obj.localScale = obj.localScale * detail;

                Cell cellClicked = grid.GetCellXY(pos);
                cellClicked.OcccupingObject = obj.gameObject;
            }

        }
        else if (Input.GetMouseButtonDown(0) && mode == Mode.Destroy)
        {
            Vector3 pos = grid.GetWorldPos(Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).y));

            if (grid.GetCellXY(pos).OcccupingObject != null && grid.GetCellXY(pos).OcccupingObject.tag != "Main")
            {
                Cell cellClicked = grid.GetCellXY(pos);
                Destroy(cellClicked.OcccupingObject);
            }
        }

        else if (Input.GetMouseButtonDown(0) && mode == Mode.Buy)
        {
            Vector3 pos = grid.GetWorldPos(Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).y));
            Cell cellClicked = grid.GetCellXY(pos);

            if (IsCellOwnable(cellClicked) && PlayerInfo.gold - 30 >= 0)
            {
                cellClicked.cellOwned = true;
                PlayerInfo.gold -= 30;
                PlayerInfo.tilesOwned += 1;
            }
        }
    }

    public void RotateWithAmount(GameObject Building, int rotationAmount)
    {
        if (RotationAmount == 0) return;

        Vector3 originalPos = Building.transform.position;

        if(rotationAmount == 90)
        {
            Building.transform.position = originalPos;
            Building.transform.rotation = Quaternion.Euler(0, 0, 90);

            if (Building.tag != "H_Road" || Building.tag != "V_Road")
            {
                Building.transform.position = new Vector3(Building.transform.position.x + 1, Building.transform.position.y, 0);
            }
            else
            {
                Building.transform.position = new Vector3(Building.transform.position.x + 1.015f, Building.transform.position.y, 0);
            }
        }

    }

    public bool IsCellOwnable(Cell cell)
    {
        if (cell.cellOwned) { return false; }

        List<Cell> n = grid.GetCellNeighborList(cell);

        foreach (Cell c in  n)
        {
            if (c.cellOwned)
            {
                return true;
            }
        }

        return false;
    }

    public enum Mode
    {
        Create,
        Destroy,
        Buy
    }

}

