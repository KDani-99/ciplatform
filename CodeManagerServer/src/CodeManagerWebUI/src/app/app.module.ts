import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ConfigService } from './config/config.service';
import { HttpGlobalInterceptor } from './interceptors/http-global.interceptor';
import { GlobalErrorHandler } from './error/global-error.handler';
import { NavbarComponent } from './navbar/navbar.component';
import { ButtonComponent } from './navbar/components/button/button.component';
import { UserComponent } from './navbar/components/user/user.component';
import { TeamComponent } from './team/modules/team/team.component';
import { TeamModule } from './team/modules/team/team.module';
import { ProjectsComponent } from './project/modules/projects/projects.component';
import { ProjectsModule } from './project/modules/projects/projects.module';
import { ProjectComponent } from './project/modules/project/project.component';
import { RunComponent } from './run/modules/run/run.component';
import { RunModule } from './run/modules/run/run.module';
import { JobComponent } from './job/modules/job/job.component';
import { PopupComponent } from './shared/popup/popup.component';
import { TeamsModule } from './team/modules/teams/teams.module';
import { TeamsComponent } from './team/modules/teams/teams.component';
import { ProjectModule } from './project/modules/project/project.module';
import { UsersComponent } from './user/modules/users/users.component';
import { UsersModule } from './user/modules/users/users.module';
import { NgxsModule } from '@ngxs/store';
import { NgxsStoragePluginModule } from '@ngxs/storage-plugin';
import { AppState } from './store/app/app.state';
import { LoginGuard } from './guards/LoginGuard';
import { JwtAuthInterceptor } from './interceptors/jwt-http-auth.inteceptor';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    ButtonComponent,
    UserComponent,
    TeamComponent,
    JobComponent,
    ProjectsComponent,
    ProjectComponent,
    TeamsComponent,
    RunComponent,
    PopupComponent,
    UsersComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ToastrModule.forRoot(),
    TeamModule,
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
