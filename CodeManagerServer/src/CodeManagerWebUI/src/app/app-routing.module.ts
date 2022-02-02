import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './modules/login/login.component';
import { RegisterComponent } from './modules/register/register.component';
import { TeamsComponent } from './modules/teams/teams.component';
import { TeamComponent } from './modules/team/team.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    loadChildren: () =>
      import('./modules/login/login.module').then((m) => m.LoginModule),
  },
  {
    path: 'register',
    component: RegisterComponent,
    loadChildren: () =>
      import('./modules/register/register.module').then(
        (m) => m.RegisterModule,
      ),
  },
  {
    path: 'teams',
    component: TeamsComponent,
    loadChildren: () =>
      import('./modules/teams/teams.module').then((m) => m.TeamsModule),
  },
  {
    path: 'teams/:id',
    component: TeamComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
