import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams, HttpUploadProgressEvent, HttpDownloadProgressEvent, HttpEvent } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { tap, catchError, takeUntil } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ActionsService } from 'src/app/shared/services/actions.service';
import { FileModel } from './model-FileModel';
import { of, Subject, Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class FilesService {

  constructor(
    private http: HttpClient,
    private toast: ToastrService,
    private actionsService: ActionsService
  ) { }

  getFiles(): Promise<FileModel[]> {
    return this.http.get<FileModel[]>(`${environment.apiUrl}/api/File/GetFiles`).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        this.toast.error(error.error.Message);
        throw new Error(error.error.Message);
      })
    ).toPromise();
  }

  uploadFiles(files: File[], userData: string, creatorId: number): Observable<HttpEvent<HttpUploadProgressEvent>> {
    const formData = new FormData();
    files.forEach((file: File) => {
      formData.append('files', file, file.name);
    });

    let params = new HttpParams();
    params = params.append('userData', userData);
    params = params.append('creatorId', creatorId.toString());

    return this.http.post<HttpUploadProgressEvent>(`${environment.apiUrl}/api/File/UploadFiles`, formData,
      { params, reportProgress: true, observe: 'events' });
  }

  deleteFiles(filesToDeleteIds: number[], userData: string): Promise<void> {
    let params = new HttpParams();
    filesToDeleteIds.forEach((x: number) => {
      params = params.append('fileIds', x.toString());
    });
    params = params.append('userData', userData);

    this.actionsService.startAction();
    return this.http.delete<void>(`${environment.apiUrl}/api/File/DeleteFiles`, { params }).pipe(
      tap(() => this.toast.success('Pomyślnie usunięto pliki')),
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        this.toast.error(error.error.Message);
        throw new Error(error.error.Message);
      })
    ).toPromise().finally(() => this.actionsService.stopAction());
  }

  downloadFile(id: number): Promise<Blob> {
    const params = new HttpParams().append('fileId', id.toString());

    return this.http.get<Blob>(`${environment.apiUrl}/api/File/DownloadFile`, { params, responseType: 'blob' as 'json' }).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        this.toast.error(error.error.Message);
        throw new Error(error.error.Message);
      })
    ).toPromise();
  }

  UpdateFile(element: FileModel): Promise<void> {
    return this.http.put<void>(`${environment.apiUrl}/api/File/UpdateFile`, element).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        throw new Error(error.error.Message);
      })
    ).toPromise();
  }

}
