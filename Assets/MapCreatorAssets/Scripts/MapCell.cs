using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Lean.Localization;

public enum MapCellType { None, Rock, Lake}
public class MapCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI ccText;
    [SerializeField] private Image image;

    [SerializeField] public MapCellType cellType = MapCellType.None;
    [SerializeField] public int credits = 0;

    [SerializeField] public HQ_place_status hq = HQ_place_status.None;
    [SerializeField] public Unit unit = Unit.None;

    [SerializeField] public int playerNo = 0;

    [SerializeField] public int x;
    [SerializeField] public int y;

    [SerializeField] public CellStats cellStats;

    private void Start()
    {
        if (cellStats == null)
            cellStats = new CellStats();

        if (unit == Unit.None && hq == HQ_place_status.None)
            text.text = cellType.ToString();

        cellStats.cellType = cellType;
        cellStats.credits = credits;
        cellStats.hq = hq;

        cellStats.x = x;
        cellStats.y = y;

        TranslateRu();
    }

    private void TranslateRu()
    {
        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
        {
            switch (text.text)
            {
                case "None":
                    text.text = "Пусто";
                    break;
                case "Rock":
                    text.text = "Горы";
                    break;
                case "Lake":
                    text.text = "Вода";
                    break;
                case "HQ_0":
                    text.text = "База";
                    break;
                case "HQ_1":
                    text.text = "База";
                    break;
                case "Tank_0":
                    text.text = "Танк";
                    break;
                case "Tank_1":
                    text.text = "Танк";
                    break;
                case "Drill_0":
                    text.text = "Бур";
                    break;
                case "Drill_1":
                    text.text = "Бур";
                    break;
                case "Plate_0":
                    text.text = "Плита";
                    break;
                case "Plate_1":
                    text.text = "Плита";
                    break;
                case "Fabric_0":
                    text.text = "Завод";
                    break;
                case "Fabric_1":
                    text.text = "Завод";
                    break;
                case "Tower_0":
                    text.text = "Башня";
                    break;
                case "Tower_1":
                    text.text = "Башня";
                    break;
                case "Helicopter_0":
                    text.text = "Верт.";
                    break;
                case "Helicopter_1":
                    text.text = "Верт.";
                    break;
            }
        }
        else
        {
            switch (text.text)
            {
                case "HQ_0":
                    text.text = "Base";
                    break;
                case "HQ_1":
                    text.text = "Base";
                    break;
            }
        }

    }

    public void OnClick()
    {
        if (MapGenerator.Instance.isCCEdit)
        {
            EditCC(MapGenerator.Instance.cc_count);
            return;
        }

        if (MapGenerator.Instance.hq == HQ_place_status.None && MapGenerator.Instance.currUnit == Unit.None)
        {
            hq = HQ_place_status.None;
            unit = Unit.None;

            switch (cellType)
            {
                case MapCellType.None:
                    cellType = MapCellType.Rock;
                    text.text = cellType.ToString();
                    image.color = new Color(0.3f, 0.3f, 0.3f);
                    break;
                case MapCellType.Rock:
                    cellType = MapCellType.Lake;
                    text.text = cellType.ToString();
                    image.color = new Color(0.1f, 0.1f, 0.9f);
                    break;
                case MapCellType.Lake:
                    cellType = MapCellType.None;
                    text.text = cellType.ToString();
                    image.color = new Color(0.8f, 0.8f, 0.65f);
                    break;
            }
        }
        else if (MapGenerator.Instance.hq != HQ_place_status.None)
        {
            unit = Unit.None;
            cellType = MapCellType.None;
            hq = MapGenerator.Instance.hq;
            if (hq == HQ_place_status.HQ_0)
                image.color = new Color(0.67f, 0.67f, 0f);
            else if (hq == HQ_place_status.HQ_1)
                image.color = new Color(1f, 0f, 0f);
            text.text = MapGenerator.Instance.hq.ToString();
        }
        else if (MapGenerator.Instance.hq == HQ_place_status.None)
        {
            cellType = MapCellType.None;
            hq = HQ_place_status.None;
            playerNo = MapGenerator.Instance.playerNo;

            unit = MapGenerator.Instance.currUnit;

            if (playerNo == 0)
                image.color = new Color(0.67f, 0.67f, 0f);
            else if (playerNo == 1)
                image.color = new Color(1f, 0f, 0f);
            text.text = MapGenerator.Instance.currUnit.ToString() + "_" + playerNo;
        }

        cellStats.cellType = cellType;
        cellStats.credits = credits;
        cellStats.hq = hq;
        cellStats.unit = unit;

        cellStats.playerNo = playerNo;

        cellStats.x = x;
        cellStats.y = y;

        TranslateRu();
    }

    public void FillCellbyStats()
    {
        hq = cellStats.hq;
        cellType = cellStats.cellType;
        credits = cellStats.credits;
        unit = cellStats.unit;

        playerNo = cellStats.playerNo;

        x = cellStats.x;
        y = cellStats.y;

        ccText.text = credits.ToString();

        switch (cellType)
        {
            case MapCellType.Rock:
                text.text = cellType.ToString();
                image.color = new Color(0.3f, 0.3f, 0.3f);
                break;
            case MapCellType.Lake:
                text.text = cellType.ToString();
                image.color = new Color(0.1f, 0.1f, 0.9f);
                break;
            case MapCellType.None:
                text.text = cellType.ToString();
                image.color = new Color(0.8f, 0.8f, 0.65f);
                break;
        }

        if (hq == HQ_place_status.HQ_0)
        {
            image.color = new Color(0.67f, 0.67f, 0f);
            text.text = hq.ToString();
            return;
        }
        else if (hq == HQ_place_status.HQ_1)
        {
            image.color = new Color(1f, 0f, 0f);
            text.text = hq.ToString();
            return;
        }

        if (unit != Unit.None)
        {
            if (playerNo == 0)
                image.color = new Color(0.67f, 0.67f, 0f);
            else if (playerNo == 1)
                image.color = new Color(1f, 0f, 0f);
            text.text = unit.ToString() + "_" + playerNo;
        }
    }

    public void EditCC(int value)
    {
        credits = value;
        cellStats.credits = credits;

        ccText.text = credits.ToString();
    }
}
