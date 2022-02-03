import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './modules/login/login.component';
import { RegisterComponent } from './modules/register/register.component';
import { TeamsComponent } from './modules/teams/teams.component';
import { TeamComponent } from './modules/team/team.component';
import { ProjectsComponent } from './modules/projects/projects.component';
import { ProjectComponent } from './modules/project/project.component';
import { RunComponent } from './modules/run/run.component';
import { JobComponent } from './modules/job/job.component';

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
  {
    path: 'projects',
    component: ProjectsComponent,
    loadChildren: () =>
      import('./modules/projects/projects.module').then(
        (m) => m.ProjectsModule,
      ),
  },
  {
    path: 'projects/:id',
    component: ProjectComponent,
  },
  {
    path: 'runs/:id',
    component: RunComponent,
  },
  {
    path: 'jobs/:id',
    component: JobComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
