import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  constructor(private authService: AuthService , private alerify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
   this.authService.register(this.model).subscribe(() => {
     this.alerify.success('registration successful');
   }, error => {
     this.alerify.error(error);
  });
  }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('Cancelled');
  }

}
