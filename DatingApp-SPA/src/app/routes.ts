import { AuthGuard } from './_guards/auth.guard';
import { Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';

export const appRoutes: Routes = [
  // Protecting multiple routes with single route guard
  /*{ path: '' , component: HomeComponent }, // This is not same as below we
                                            // put nothing then it will take us to home but
                                            // if we put anything after / it will take us to the children
  {
    path: '',  // localhost:4200/member
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [

      { path: 'members' , component: MemberListComponent , canActivate: [AuthGuard] },
      { path: 'messages' , component: MessagesComponent , canActivate: [AuthGuard] },
      { path: 'lists' , component: ListsComponent , canActivate: [AuthGuard] },
    ]
  },

  { path: '**', redirectTo: '' , pathMatch: 'full' }*/   // Wild card route work same as home component

  // Protect muliple routes with multiple guard
  { path: 'home', component: HomeComponent },
  // { path: '', component: HomeComponent },
  { path: 'members', component: MemberListComponent, canActivate: [AuthGuard]},
  { path: 'messages', component: MessagesComponent, canActivate: [AuthGuard]},
  { path: 'lists', component: ListsComponent, canActivate: [AuthGuard]},
  { path: '**' , redirectTo: 'home', pathMatch: 'full'}
  // { path: '**' , redirectTo: '', pathMatch: 'full'}
];

