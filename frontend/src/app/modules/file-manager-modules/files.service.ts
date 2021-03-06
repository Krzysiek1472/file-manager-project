import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams, HttpUploadProgressEvent, HttpDownloadProgressEvent, HttpEvent } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { tap, catchError, takeUntil } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ActionsService } from 'src/app/shared/services/actions.service';
import { FileModel } from './model-FileModel';
import { of, Subject, Observable } from 'rxjs';
import { translations } from 'src/app/app.component';
import { ShareableLinkModel } from '../file-share-modules/model-ShareableLinkModel';


@Injectable({
  providedIn: 'root'
})
export class FilesService {
  constructor(
    private http: HttpClient,
    private toast: ToastrService,
    private actionsService: ActionsService
  ) { }

  getAllFiles(): Promise<FileModel[]> {
    return this.http.get<FileModel[]>(`${environment.apiUrl}/api/File/GetAllFiles`).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if (error.error.Data?.message) {
          const messageTranslateCode = error.error.Data.message;
          this.toast.error(translations[messageTranslateCode]);
        } else {
          this.toast.error(translations.GENERAL_HTTP_ERROR);
        }
        throw new Error();
      })
    ).toPromise();
  }

  getFilesInsideFolder(folderId: number): Promise<FileModel[]> {
    const params = new HttpParams().append('folderId', folderId.toString());

    return this.http.get<FileModel[]>(`${environment.apiUrl}/api/File/GetFilesInsideFolder`, { params }).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if (error.error.Data?.message) {
          const messageTranslateCode = error.error.Data.message;
          this.toast.error(translations[messageTranslateCode]);
        } else {
          this.toast.error(translations.GENERAL_HTTP_ERROR);
        }
        throw new Error();
      })
    ).toPromise();
  }

  uploadFiles(files: File[], userData: string, creatorId: number, folderId: number): Observable<HttpEvent<HttpUploadProgressEvent>> {
    const formData = new FormData();
    files.forEach((file: File) => {
      formData.append('files', file, file.name);
    });

    let params = new HttpParams();
    params = params.append('userData', userData);
    params = params.append('creatorId', creatorId.toString());
    params = params.append('folderId', folderId.toString());

    return this.http.post<HttpUploadProgressEvent>(`${environment.apiUrl}/api/File/UploadFiles`, formData,
      { params, reportProgress: true, observe: 'events' });
  }

  deleteFiles(filesToDeleteIds: number[], userData: string): Promise<void> {
    let params = new HttpParams();
    filesToDeleteIds.forEach((x: number) => {
      params = params.append('fileIds', x.toString());
    });
    params = params.append('userData', userData);

    this.actionsService.startBackendAction();
    return this.http.delete<void>(`${environment.apiUrl}/api/File/DeleteFiles`, { params }).pipe(
      tap(() => this.toast.success(translations.FILES_DELETE_SUCCESS)),
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if (error.error.Data?.message) {
          const messageTranslateCode = error.error.Data.message;
          this.toast.error(translations[messageTranslateCode]);
        } else {
          this.toast.error(translations.GENERAL_HTTP_ERROR);
        }
        throw new Error();
      })
    ).toPromise().finally(() => this.actionsService.stopBackendAction());
  }

  downloadFile(id: number): Observable<HttpEvent<HttpDownloadProgressEvent>> {
    const params = new HttpParams().append('fileId', id.toString());

    return this.http.get<HttpDownloadProgressEvent>(`${environment.apiUrl}/api/File/DownloadFile`,
      { params, responseType: 'blob' as 'json', observe: 'events', reportProgress: true });
  }

  updateFile(element: FileModel): Promise<void> {
    return this.http.put<void>(`${environment.apiUrl}/api/File/UpdateFile`, element).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        throw new Error();
      })
    ).toPromise();
  }

  createShareableLink(fileId: number, filePassword?: string): Promise<string> {
    let params = new HttpParams();
    params = params.append('fileId', fileId.toString());
    if (filePassword) {
      params = params.append('filePassword', filePassword);
    }

    return this.http.post<string>(`${environment.apiUrl}/api/File/CreateShareableLink`, {}, { params }).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if (error.error.Data?.message) {
          const messageTranslateCode = error.error.Data.message;
          this.toast.error(translations[messageTranslateCode]);
        } else {
          this.toast.error(translations.GENERAL_HTTP_ERROR);
        }
        throw new Error();
      })
    ).toPromise();
  }

  getShareableLink(linkGuidId: string): Promise<ShareableLinkModel> {
    const params = new HttpParams().append('linkGuidId', linkGuidId);

    return this.http.get<ShareableLinkModel>(`${environment.apiUrl}/api/File/GetShareableLink`, { params }).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if (error.error.Data?.message) {
          const messageTranslateCode = error.error.Data.message;
          this.toast.error(translations[messageTranslateCode]);
        } else {
          this.toast.error(translations.GENERAL_HTTP_ERROR);
        }
        throw new Error();
      })
    ).toPromise();
  }

  getSpaceOccupiedByFiles(): Promise<number> {
    return this.http.get<number>(`${environment.apiUrl}/api/File/GetSpaceOccupiedByFiles`).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if (error.error.Data?.message) {
          const messageTranslateCode = error.error.Data.message;
          this.toast.error(translations[messageTranslateCode]);
        } else {
          this.toast.error(translations.GENERAL_HTTP_ERROR);
        }
        throw new Error();
      })
    ).toPromise();
  }

}
