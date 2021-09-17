using System.Collections;
using UnityEngine;

using GameplayFeature.Focus.Manager;

namespace GameplayFeature.Manager
{
    public class Main : GameplayFeature 
    {
        //FocusManager _focusManager = new FocusManager();
    }
}

public class Main : MonoBehaviour
{
    System.Random _random;

    static Main _main;

    [SerializeField]
    GameObject _sturdyBot;

    [SerializeField]
    GameObject[] _monsterBot;

    GameObject _currentFocus;
    int _currentMonsterBotFocus, _lastMonsterBotFocus;

    public float[] distance;

    Vector3[] _originalPosition;

    public GameObject GetCurrentFocus => _currentFocus;

    public static Main GetInstance => _main;

    void Awake()
    {
        _main = this;

        _random = new System.Random();
    }

    // Start is called before the first frame update
    void Start()
    {
        distance = new float[_monsterBot.Length];
        _originalPosition = new Vector3[_monsterBot.Length];

        for (int i = 0; i < _monsterBot.Length; ++i)
        {
            distance[i] = Vector3.Distance(_monsterBot[i].transform.position, _sturdyBot.transform.position);

            _originalPosition[i] = _monsterBot[i].transform.position;
        }

        StartCoroutine(SetNextFocus());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SetNextFocus() 
    {
        while (true)
        {
            _currentMonsterBotFocus = _random.Next(_monsterBot.Length);

            while (_currentMonsterBotFocus == _lastMonsterBotFocus)
                _currentMonsterBotFocus = _random.Next(_monsterBot.Length);

            _lastMonsterBotFocus = _currentMonsterBotFocus;

            _monsterBot[_currentMonsterBotFocus].transform.position = Vector3.MoveTowards(_monsterBot[_currentMonsterBotFocus].transform.position, _sturdyBot.transform.position, 1f);

            _currentFocus = _monsterBot[_currentMonsterBotFocus];

            yield return new WaitForSeconds(1f);

            _monsterBot[_currentMonsterBotFocus].transform.position = _originalPosition[_currentMonsterBotFocus];
        }
    }
}