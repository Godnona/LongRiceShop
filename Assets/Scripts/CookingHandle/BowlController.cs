using UnityEngine;

public class BowlController : MonoBehaviour
{
    [SerializeField] GameObject comTrung;
    [SerializeField] GameObject comSuon;
    [SerializeField] GameObject trungSuon;

    [SerializeField] GameObject comCa;
    [SerializeField] GameObject comThitKho;
    [SerializeField] GameObject comGa;
    [SerializeField] GameObject comDacBiet;

    [SerializeField] GameObject com;
    [SerializeField] GameObject trung;
    [SerializeField] GameObject suon;

    [SerializeField] GameObject cachua;
    [SerializeField] GameObject ca;
    [SerializeField] GameObject bongcai;
    [SerializeField] GameObject thitkho;
    [SerializeField] GameObject ga;
    [SerializeField] GameObject so;
    [SerializeField] GameObject tomhum;


    public Rigidbody rb;
    public Collider pickupCollider;

    public BowlState CurrentState { get; private set; } = BowlState.Empty;

    void Awake()
    {
        ResetBowl();
        rb.isKinematic = true;
        pickupCollider.enabled = false;
    }

    void Start()
    {
        PopupManager.Instance.HidePopup(PopupName.PopupMaps);
        ChangeEmptyState();
    }

    public void ChangeState(BowlState state)
    {
        CurrentState = state;

        // combo
        comTrung.SetActive(state == BowlState.ComTrung);
        comSuon.SetActive(state == BowlState.ComSuon);
        trungSuon.SetActive(state == BowlState.TrungSuon);

        comGa.SetActive(state == BowlState.ComGa);
        comThitKho.SetActive(state == BowlState.ComThitKho);
        comCa.SetActive(state == BowlState.ComCa);
        comDacBiet.SetActive(state == BowlState.ComDacBiet);

        // món đơn cũ
        com.SetActive(state == BowlState.Com);
        trung.SetActive(state == BowlState.Trung);
        suon.SetActive(state == BowlState.Suon);

        // ⭐ món đơn mới
        ca.SetActive(state == BowlState.Ca);
        cachua.SetActive(state == BowlState.CaChua);
        bongcai.SetActive(state == BowlState.BongCai);
        thitkho.SetActive(state == BowlState.ThitKho);
        ga.SetActive(state == BowlState.Ga);
        so.SetActive(state == BowlState.So);
        tomhum.SetActive(state == BowlState.TomHum);
    }

    // Món đơn cũ
    public void ChangeEmptyState() => ChangeState(BowlState.Empty);
    public void ChangeComTrungState() => ChangeState(BowlState.ComTrung);
    public void ChangeComSuonState() => ChangeState(BowlState.ComSuon);
    public void ChangeComState() => ChangeState(BowlState.Com);

    // Món cũ
    public void ChangeTrungState() => ChangeState(BowlState.Trung);
    public void ChangeSuonState() => ChangeState(BowlState.Suon);

    // Món đơn mới
    public void ChangeCaState() => ChangeState(BowlState.Ca);
    public void ChangeCaChuaState() => ChangeState(BowlState.CaChua);
    public void ChangeBongCaiState() => ChangeState(BowlState.BongCai);
    public void ChangeThitKhoState() => ChangeState(BowlState.ThitKho);
    public void ChangeGaState() => ChangeState(BowlState.Ga);
    public void ChangeSoState() => ChangeState(BowlState.So);
    public void ChangeTomHumState() => ChangeState(BowlState.TomHum);

    // Món mới
    public void ChangeComGaState() => ChangeState(BowlState.ComGa);
    public void ChangeComThitKhoState() => ChangeState(BowlState.ComThitKho);
    public void ChangeComCaState() => ChangeState(BowlState.ComCa);
    public void ChangeComDacBietState() => ChangeState(BowlState.ComDacBiet);

    public void ShowCom() => com.SetActive(true);
    public void ShowTrung() => trung.SetActive(true);
    public void ShowSuon() => suon.SetActive(true);
    public void ShowCa() => ca.SetActive(true);
    public void ShowCaChua() => cachua.SetActive(true);
    public void ShowBongCai() => bongcai.SetActive(true);
    public void ShowThitKho() => thitkho.SetActive(true);
    public void ShowGa() => ga.SetActive(true);
    public void ShowSo() => so.SetActive(true);
    public void ShowTomHum() => tomhum.SetActive(true);


    public void ResetBowl()
    {
        ChangeState(BowlState.Empty);
    }

    public void EnablePickup(BowlState cookedState)
    {
        ChangeState(cookedState);

        rb.isKinematic = false;
        rb.useGravity = false;
        pickupCollider.enabled = true;
        pickupCollider.isTrigger = true;

        var dish = GetComponent<DishPickup>();
        if (!dish) dish = gameObject.AddComponent<DishPickup>();
        dish.Init(cookedState);
    }
}
