import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ConfigService } from './config/config.service';
import { HttpGlobalInterceptor } from './interceptors/http-global.interceptor';
import { GlobalErrorHandler } from './error/global-error.handler';
import { NavbarComponent } from './components/navbar/navbar.component';
import { ButtonComponent } from './components/navbar/components/button/button.component';
import { UserComponent } from './components/navbar/components/user/user.component';
import { TeamComponent } from './pages/team/modules/team/team.component';
import { TeamModule } from './pages/team/modules/team/team.module';
import { ProjectsComponent } from './pages/project/modules/projects/projects.component';
import { ProjectsModule } from './pages/project/modules/projects/projects.module';
import { ProjectComponent } from './pages/project/modules/project/project.component';
import { RunComponent } from './pages/run/modules/run/run.component';
import { RunModule } from './pages/run/modules/run/run.module';
import { JobComponent } from './pages/job/modules/job/job.component';
import { PopupComponent } from './shared/popup/popup.component';
import { TeamsModule } from './pages/team/modules/teams/teams.module';
import { TeamsComponent } from './pages/team/modules/teams/teams.component';
import { ProjectModule } from './pages/project/modules/project/project.module';
import { UsersComponent } from './pages/user/modules/users/users.component';
import { UsersModule } from './pages/user/modules/users/users.module';
import { NgxsModule } from '@ngxs/store';
import { NgxsStoragePluginModule } from '@ngxs/storage-plugin';
import { AppState } from './state/app/app.state';
import { AuthenticatedGuard } from './guards/authenticated.guard';
import { JwtAuthInterceptor } from './interceptors/jwt-http-auth.inteceptor';
import { LoginGuard } from './guards/login.guard';
import { ErrorComponent } from './components/error/error.component';
import { JobModule } from './pages/job/modules/job/job.module';

@NgModule({
  declarations: [
    ErrorComponent,
    AppComponent,
    NavbarComponent,
    ButtonComponent,
    UserComponent,
    TeamComponent,
    ProjectsComponent,
    ProjectComponent,
    TeamsComponent,
    RunComponent,
    PopupComponent,
    JobComponent,
    UsersComponent,
    ErrorComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    BrowserAnimationsModule,
    HttpClientModule,
    TeamModule,
    JobModule,
    ProjectsModule,
    TeamsModule,
    RunModule,
    ProjectModule,
    UsersModule,
    NgxsModule.forRoot([AppState]),
    NgxsStoragePluginModule.forRoot({
      key: AppState,
    }),
  ],
  providers: [
    AuthenticatedGuard,
    LoginGuard,
    {
      provide: APP_INITIALIZER,
      multi: true,
      deps: [ConfigService],
      useFactory: (configService: ConfigService) => {
        return () => {
          return configService.loadConfig();
        };
      },
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpGlobalInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtAuthInterceptor,
      multi: true,
    },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
  ],
  exports: [PopupComponent],
  bootstrap: [AppComponent],
})
export class AppModule {}
