using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WeaponEnhance : MonoBehaviour
{
    public Text levelText;
    public Text expText;
    public Slider expSlider;
    public Text resultText;

    private int currentLevel = 1;
    private int requiredExp = 0;

    class Item { public string name; public int exp; public int price; }

    private List<Item> items = new List<Item>()
    {
        new Item { name = "강화석 소", exp = 3, price = 8 },
        new Item { name = "강화석 중", exp = 5, price = 12 },
        new Item { name = "강화석 대", exp = 12, price = 30 },
        new Item { name = "강화석 특대", exp = 20, price = 45 }
    };

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        int targetLevel = currentLevel + 1;
        requiredExp = 8 * (targetLevel * targetLevel);

        levelText.text = "+" + currentLevel + "  ->  +" + targetLevel;
        expText.text = "필요 경험치 0 / " + requiredExp;

        expSlider.maxValue = requiredExp;
        expSlider.value = 0;
    }

    public void Click_LevelUp()
    {
        if (expSlider.value >= expSlider.maxValue && expSlider.maxValue > 0)
        {
            currentLevel++; 

       
            levelText.color = new Color(Random.value, Random.value, Random.value);

            resultText.text = "강화 성공! 레벨이 올랐습니다.";

            UpdateUI(); 
        }
        else
        {
            resultText.text = "경험치가 부족합니다! 재료를 먼저 구매하세요.";
        }
    }


    public void Click_BruteForce() { RunAlgo(1); }
    public void Click_MinWaste() { RunAlgo(2); }
    public void Click_MaxEfficiency() { RunAlgo(3); }
    public void Click_BigFirst() { RunAlgo(4); }

    void RunAlgo(int type)
    {
        int cost = 0; int exp = 0; string log = "";

        if (type == 1) 
        {
            int max = requiredExp + 20;
            int[] dp = Enumerable.Repeat(99999, max + 1).ToArray();
            string[] path = new string[max + 1];
            dp[0] = 0; path[0] = "";
            for (int i = 0; i <= max; i++)
            {
                if (dp[i] == 99999) continue;
                foreach (var item in items)
                {
                    int n = i + item.exp;
                    if (n <= max && dp[n] > dp[i] + item.price) { dp[n] = dp[i] + item.price; path[n] = path[i] + "," + item.name; }
                }
            }
            cost = 99999; int best = -1;
            for (int i = requiredExp; i <= max; i++) if (dp[i] < cost) { cost = dp[i]; best = i; }
            exp = best; log = path[best];
        }
        else 
        {
            List<Item> sorted = items;
            if (type == 2) sorted = items.OrderByDescending(x => x.exp).ToList();
            if (type == 3) sorted = items.OrderByDescending(x => (float)x.exp / x.price).ToList();
            if (type == 4) sorted = items.OrderByDescending(x => x.exp).ToList();

            while (exp < requiredExp)
            {
                if (type == 4)
                { 
                    foreach (var item in sorted)
                    {
                        while (exp + item.exp <= requiredExp) { exp += item.exp; cost += item.price; log += "," + item.name; }
                    }
                    if (exp < requiredExp) { var min = items.Last(); exp += min.exp; cost += min.price; log += "," + min.name; }
                    break;
                }

        
                bool bought = false;
                foreach (var item in sorted)
                {
                    if (exp + item.exp <= requiredExp)
                    {
                        exp += item.exp; cost += item.price; log += "," + item.name;
                        bought = true; break; 
                    }
                }
                if (!bought)
                { 
                    var pick = (type == 3) ? sorted[0] : items.Last();
                    exp += pick.exp; cost += pick.price; log += "," + pick.name;
                }
            }
        }

 
        ShowResult(type, cost, exp, log);
    }

    void ShowResult(int type, int cost, int finalExp, string history)
    {
        var names = history.Split(',').Where(x => x.Length > 0);
        var counts = names.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        string t = type == 1 ? "Brute Force" : type == 2 ? "낭비 최소" : type == 3 ? "효율 최대" : "큰 거 부터";

        string txt = $"[{t}]\n";
        foreach (var p in counts) txt += $"{p.Key} x {p.Value}\n";
        txt += $"\n총 경험치: {finalExp} / {requiredExp}\n총 가격: {cost} Gold";

        resultText.text = txt;

      
        expSlider.value = requiredExp;
        expText.text = $"필요 경험치 {requiredExp} / {requiredExp}";
    }
}