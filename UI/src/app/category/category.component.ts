import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Category } from 'app/models/category';
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
  val: Category = this.category;
  dataSource = new MatTableDataSource();
  columns: string[] = ['id', 'categoryName', 'action'];
  baseAddress = "https://localhost:7024/api/Category";
  @ViewChild('fileInput') fileInput: ElementRef;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private httpClient: HttpClient) { }
  ngOnInit() {
    this.get();
  }

  loadData(data:any){
    this.dataSource = new MatTableDataSource<any>(data);
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  get(){
    this.httpClient.get(this.baseAddress)
    .subscribe((res: Category[])=>{
      this.categories = res;
      this.loadData(this.categories);
    });
  }

  edit(category: Category){
    this.val.categoryName = category.categoryName;
    console.log(this.val);
    category.isEditEnabled = true;
  }
  close(category: Category){
    console.log(this.val);
    category.categoryName = this.val.categoryName;
    category.isEditEnabled = false;
    // this.loadData(this.categories);

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
    this.loading = !this.loading;
    this.upload(this.file).subscribe( 
      (res: any) => {
        this.notify(res.message, true);
        this.file = null;
        this.fileInput.nativeElement.value = "";
        this.get();
      },
      error=>{
        console.log(error);
      } 
    ); 
  } 

  upload(file):Observable<any> {
    const formData = new FormData();  
    formData.append("file", file, file.name); 
    return this.httpClient.post(this.baseAddress + '/bulk' , formData) 
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
