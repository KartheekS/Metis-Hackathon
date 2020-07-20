import { Component } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http'

@Component({
  selector: 'upload-component',
  templateUrl: './upload.component.html'
})
export class UploadComponent {
  public progress: number;
  public message: string;
  public forecast: string = 'Upload a audio file';
  public forecasts: string = '';
  //public toShow: boolean = false;
  constructor(private http: HttpClient) { }

  upload(files) {
    if (files.length === 0)
      return;
    this.forecast = '';
    this.forecasts = '';
    //this.toShow = true;
    

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', `api/upload`, formData, {
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(result => {
      if (result.type === HttpEventType.Response)
        this.forecasts = result.body.toString();
     // console.log(this.forecasts);
      //  this.toShow = false;
    }, error => console.error(error));
    
  }
}
