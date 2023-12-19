import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Category } from 'app/models/category';
import { Pagination } from 'app/models/pagination';
import { Observable } from 'rxjs';
declare var $: any;


@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css'],
})
export class CategoryComponent implements OnInit {
  loading: boolean = false;
  file: File = null;
  categories: Category[];
  category: Category = {
    categoryName: ''
  };
  val: Category = {
    categoryName: ''
  }
  dataSource = new MatTableDataSource();
  columns: string[] = ['id', 'categoryName', 'action'];
  baseAddress = "Category";
  @ViewChild('fileInput') fileInput: ElementRef;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  pagination = new Pagination();

  constructor(private httpClient: HttpClient) { }
  ngOnInit() {
    this.get();
  }

  loadData(data:any){
    this.dataSource = new MatTableDataSource<any>(data);
    this.dataSource.sort = this.sort;
  }

  get(){
    this.httpClient.get(this.baseAddress+'/page?index='+this.pagination.pageIndex + '&size=' + this.pagination.pageSize)
    .subscribe((res: any)=>{
      this.categories = res.data;
      this.pagination.totalCount = res.totalCount;
      this.loadData(this.categories);
    });
  }

  edit(category: Category){
    this.val.categoryName = category.categoryName;
    category.isEditEnabled = true;
  }
  close(category: Category){
    category.categoryName = this.val.categoryName;
    category.isEditEnabled = false;

  }
  add(){
    this.httpClient.post(this.baseAddress, this.category)
    .subscribe(
      res=> {
        this.notify("Added successfully", true);
        this.category = {
          categoryName: ''
        };
        this.get();
      },
      error=>{
        this.notify("Failed to add. " + error.error, false);
    });
  }

  save(category){
    category.isEditEnabled = false;
    this.httpClient.put(this.baseAddress+'/'+ category.id, category)
    .subscribe(
      _=> {
        this.notify("Updated successfully", true);
        this.get();
      },
      error=>{
        this.notify("Failed to update. " + error.error, false);
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
        this.get();
        this.loading = false;
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
      message: message
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
