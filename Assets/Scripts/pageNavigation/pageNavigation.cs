using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Superpow;

public class pageNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    private int unlockLevel;
    private int currentPage;
    private float val;
    public GameObject buttonGroup;
    public GameObject pageNumber;
    private float targetXPosition;
    void Start()
    {   
        currentPage = 1;
        unlockLevel = LevelController.GetUnlockLevel(); 
        currentPage = unlockLevel / 20 + 1;  
        pageNumber.SetText(currentPage.ToString());
        // base.gameObject.SetText(CurrencyController.GetBalance().ToString());

    }
    // Update is called once per frame
    void Update()
    {
        if(currentPage == 15){
            pageNumber.SetText(currentPage.ToString());
        }
        pageNumber.SetText(currentPage.ToString());
        targetXPosition = - (currentPage - 1) * 1000 - 395;
        buttonGroup.transform.localPosition = Vector3.Lerp(buttonGroup.transform.localPosition, new Vector3(targetXPosition, buttonGroup.transform.localPosition.y, buttonGroup.transform.localPosition.z), Time.deltaTime * 5f);
    }
    public void OnNextButtonClick(){
        if(currentPage == 15){
            
            currentPage = 15; 
        } else {
            currentPage++; 
        }
    }
    public void OnPreviousButtonClick(){
        if(currentPage == 1){
            currentPage = 1;
        } else {
            currentPage--;
        }
    }
    
}