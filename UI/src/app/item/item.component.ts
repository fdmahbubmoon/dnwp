import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Category } from 'app/models/category';
import { Item } from 'app/models/item';
declare var $: any;


@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {

  items: Item[];
  categories: Category[];
  item: Item = {
    itemName: '',
    categoryId: null,
    itemQuantity: null,
    itemUnit: ''
  };
  baseAddress = "https://localhost:7024/api/Item";

  constructor(private httpClient: HttpClient) { }
  ngOnInit() {
    this.getCategories();
    this.get();
  }

  getCategories(){
    this.httpClient.get('https://localhost:7024/api/Category')
    .subscribe((res: Category[])=>{
      this.categories = res;
      console.log(this.categories);
    });
  }

  get(){
    this.httpClient.get(this.baseAddress)
    .subscribe((res: Item[])=>{
      this.items = res;
      // this.items.forEach(item=>{
      //   item.categoryName = item.categoryName ?? this.categories.find(f=>f.id==item.categoryId).categoryName
      // });
    });
  }

  edit(item: Item){
    item.isEditEnabled = true;
  }
  close(item: Item){
    item.isEditEnabled = false;
  }
  add(){
    this.httpClient.post(this.baseAddress, this.item)
    .subscribe(
      res=> {
        this.notify("Added successfully", true);
        this.item = {
          itemName: '',
          categoryId: null,
          itemQuantity: null,
          itemUnit: ''
        };
        this.get();
      },
      error=>{
        this.notify("Failed to add. " + error.error, false);
        this.get();
    });
  }

  save(item){
    
    item.isEditEnabled = false;
    this.httpClient.put(this.baseAddress+'/'+ item.id, item)
    .subscribe(
      res=> {
        this.notify("Updated successfully", true);
        this.get();
      },
      error=>{
        this.notify("Failed to update. " + error.error, false);
        this.get();
    });

  }

  notify(message, isSuccess){
    $.notify({
      icon: "notifications",
      message: message,
      autoHide: true,
    },
    {
      type: isSuccess ? 'success' : 'danger',
      timer: 1000,
      placement: {
          from: 'top',
          align: 'right'
      },
    });
  }



}
