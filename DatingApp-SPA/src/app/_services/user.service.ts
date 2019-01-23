import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from './../_models/user';

const httpOptions = {
   headers: new HttpHeaders({
     'Authorization': 'Bearer ' + localStorage.getItem('token')
   })
};

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private httpclient: HttpClient) { }


  getUsers(): Observable<User[]> {
    return this.httpclient.get<User[]>(this.baseUrl + 'users');
  }

  getUser(id: any): Observable<User> {
    return this.httpclient.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.httpclient.put(this.baseUrl + 'users/' + id , user);
  }

  setMainPhoto(userId: number , id: number) {
    return this.httpclient.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain',{});
  }

  deletePhoto(userId: number , id: number) {
    return this.httpclient.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

}
