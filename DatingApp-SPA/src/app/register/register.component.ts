import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';



@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  colorTheme = 'theme-red';
  showweek = true;
  user: User;
  // @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;
  constructor(private authService: AuthService, private alertify: AlertifyService , private fb: FormBuilder , private router: Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass:this.colorTheme,
      showWeekNumbers: this.showweek,      
    },
  
    
    // 1st Approach
    this.createRegisterForm();
    // 2 Approach
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmpassword: new FormControl('', Validators.required)
    // },this.passwordMatchValidator);
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['',Validators.required],
      dateOfBirth: [null, Validators.required],
      knownAs: ['',Validators.required],
      city: ['', Validators.required],
      country: ['',Validators.required],
      password: ['' , [Validators.required , Validators.minLength(4) , Validators.maxLength(8)]],
      confirmpassword: ['',Validators.required]
    }, { validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmpassword').value ? null : { 'mismatch': true };
  }

  register() {
    if(this.registerForm.valid)
    {
      this.user = Object.assign({},this.registerForm.value);
      this.authService.register(this.user).subscribe(() =>{
        this.alertify.success('Registration successful');

      },error =>{
        this.alertify.error(error);
      }, () =>{
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });      
    }
    //  this.authService.register(this.model).subscribe(() => {
    //    this.alertify.success('registration successful');
    //  }, error => {
    //    this.alertify.error(error);
    // });
    // console.log(this.registerForm.value);
  }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('Cancelled');
  }

}
