import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Category } from 'app/models/category';
import { Item } from 'app/models/item';
import { Pagination } from 'app/models/pagination';
import { Observable } from 'rxjs';
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
  val: Item = {
    itemName: '',
    categoryId: null,
    itemQuantity: null,
    itemUnit: ''
  };;
  file: File = null;
  loading: boolean = false;
  baseAddress = "Item";
  dataSource = new MatTableDataSource();
  columns: string[] = ['id', 'itemName', 'itemUnit', 'itemQuantity', 'categoryName', 'action'];
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('fileInput') fileInput: ElementRef;
  pagination = new Pagination();

  constructor(private httpClient: HttpClient) { }
  ngOnInit() {
    this.getCategories();
    this.get();
  }

  loadData(data:any){
    this.dataSource = new MatTableDataSource<any>(data);
    this.dataSource.sort = this.sort;
  }

  getCategories(){
    this.httpClient.get('Category')
    .subscribe((res: Category[])=>{
      this.categories = res;
    });
  }

  get(){
    this.httpClient.get(this.baseAddress+'/page?index='+this.pagination.pageIndex + '&size=' + this.pagination.pageSize)
    .subscribe((res: any)=>{
      this.items = res.data;
      this.pagination.totalCount = res.totalCount;
      this.loadData(this.items);
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

  onChange(event) { 
    this.file = event.target.files[0]; 
  } 

  onUpload() { 
    this.loading = true;
    this.upload(this.file).subscribe( 
      (res: any) => {
        this.notify(res.message, true);
        this.file = null;
        this.fileInput.nativeElement.value = "";
        this.loading = false;
        this.get();
      },
      error=>{
        console.error(error);
      } 
    ); 
  } 

  upload(file):Observable<any> {
    const formData = new FormData();  
    formData.append("file", file, file.name); 
    return this.httpClient.post(this.baseAddress + '/bulk' , formData) 
  }

  changePage($event){
    this.pagination.pageSize = $event.pageSize;
    this.pagination.pageIndex = $event.pageIndex;
    this.get();
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
