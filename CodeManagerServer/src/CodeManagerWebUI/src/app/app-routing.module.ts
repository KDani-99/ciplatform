import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/modules/login/login.component';
import { RegisterComponent } from './pages/auth/modules/register/register.component';
import { TeamsComponent } from './pages/team/modules/teams/teams.component';
import { TeamComponent } from './pages/team/modules/team/team.component';
import { ProjectsComponent } from './pages/project/modules/projects/projects.component';
import { ProjectComponent } from './pages/project/modules/project/project.component';
import { RunComponent } from './pages/run/modules/run/run.component';
import { JobComponent } from './pages/job/modules/job/job.component';
import { UsersComponent } from './pages/user/modules/users/users.component';
import { AuthenticatedGuard } from './guards/authenticated.guard';
import { LoginGuard } from './guards/login.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: LoginComponent,
    loadChildren: () =>
      import('./pages/auth/modules/login/login.module').then(
        (m) => m.LoginModule,
      ),
    canActivate: [LoginGuard],
  },
  {
    path: 'register',
    component: RegisterComponent,
    loadChildren: () =>
      import('./pages/auth/modules/register/register.module').then(
        (m) => m.RegisterModule,
      ),
    canActivate: [LoginGuard],
  },
  {
    path: 'teams',
    component: TeamsComponent,
    loadChildren: () =>
      import('./pages/team/modules/teams/teams.module').then(
        (m) => m.TeamsModule,
      ),
    canActivate: [AuthenticatedGuard],
  },
  {
    path: 'users',
    component: UsersComponent,
    loadChildren: () =>
      import('./pages/user/modules/users/users.module').then(
        (m) => m.UsersModule,
      ),
    canActivate: [AuthenticatedGuard],
  },
  {
    path: 'teams/:id',
    component: TeamComponent,
    loadChildren: () =>
      import('./pages/team/modules/teams/teams.module').then(
        (m) => m.TeamsModule,
      ),
    canActivate: [AuthenticatedGuard],
  },
  {
    path: 'projects',
    component: ProjectsComponent,
    loadChildren: () =>
      import('./pages/project/modules/projects/projects.module').then(
        (m) => m.ProjectsModule,
      ),
    canActivate: [AuthenticatedGuard],
  },
  {
    path: 'projects/:id',
    component: ProjectComponent,
    canActivate: [AuthenticatedGuard],
  },
  {
    path: 'runs/:id',
    component: RunComponent,
    canActivate: [AuthenticatedGuard],
  },
  {
    path: 'runs/:runId/jobs/:id',
    component: JobComponent,
    canActivate: [AuthenticatedGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
