import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/modules/login/login.component';
import { RegisterComponent } from './auth/modules/register/register.component';
import { TeamsComponent } from './team/modules/teams/teams.component';
import { TeamComponent } from './team/modules/team/team.component';
import { ProjectsComponent } from './project/modules/projects/projects.component';
import { ProjectComponent } from './project/modules/project/project.component';
import { RunComponent } from './run/modules/run/run.component';
import { JobComponent } from './job/modules/job/job.component';
import { UsersComponent } from './user/modules/users/users.component';
import { LoginGuard } from './guards/LoginGuard';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    loadChildren: () =>
      import('./auth/modules/login/login.module').then((m) => m.LoginModule),
  },
  {
    path: 'register',
    component: RegisterComponent,
    loadChildren: () =>
      import('./auth/modules/register/register.module').then(
        (m) => m.RegisterModule,
      ),
  },
  {
    path: 'teams',
    component: TeamsComponent,
    loadChildren: () =>
      import('./team/modules/teams/teams.module').then((m) => m.TeamsModule),
    canActivate: [LoginGuard],
  },
  {
    path: 'users',
    component: UsersComponent,
    loadChildren: () =>
      import('./user/modules/users/users.module').then((m) => m.UsersModule),
    canActivate: [LoginGuard],
  },
  {
    path: 'teams/:id',
    component: TeamComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'projects',
    component: ProjectsComponent,
    loadChildren: () =>
      import('./project/modules/projects/projects.module').then(
        (m) => m.ProjectsModule,
      ),
    canActivate: [LoginGuard],
  },
  {
    path: 'projects/:id',
    component: ProjectComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'runs/:id',
    component: RunComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'jobs/:id',
    component: JobComponent,
    canActivate: [LoginGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
