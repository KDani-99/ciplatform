import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { TeamService } from '../../../../services/team/team.service';
import {
  CreateTeamDto,
  MemberDto,
  MemberPermission,
  TeamDataDto,
  UpdateRoleDto,
} from '../../../../services/team/team.interface';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CreateProjectDto,
  ProjectDto,
} from '../../../../services/project/project.interface';
import { ProjectService } from '../../../../services/project/project.service';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  currentTemplate?: TemplateRef<any>;

  @ViewChild('createProjectTemplate') createProjectTemplate?: TemplateRef<any>;
  @ViewChild('editTeamTemplate') editTeamTemplate?: TemplateRef<any>;
  @ViewChild('deleteTeamTemplate') deleteTeamTemplate?: TemplateRef<any>;
  @ViewChild('kickMemberTemplate') kickMemberTemplate?: TemplateRef<any>;
  @ViewChild('addMemberTemplate') addMemberTemplate?: TemplateRef<any>;
  @ViewChild('updateRoleTemplate') updateRoleTemplate?: TemplateRef<any>;

  @Select((state: any) => state.app.user.user.id) userId?: Observable<number>;
  @Select((state: any) => state.app.user.user.isAdmin)
  isAdmin?: Observable<boolean>;

  team!: TeamDataDto;
  selectedMember?: MemberDto;
  showWindow: boolean = false;

  constructor(
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly teamService: TeamService,
    private readonly projectService: ProjectService,
  ) {}

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.teamService.getTeam(id).subscribe({
      next: (teamDataDto: TeamDataDto) => {
        if (!teamDataDto) {
          this.router.navigate(['teams']);
          return;
        }
        this.team = teamDataDto;
      },
    });
  }

  openCreateProjectWindow(): void {
    this.toggleWindow(true, this.createProjectTemplate);
  }

  onCreateProject(createProjectDto: CreateProjectDto): void {
    createProjectDto.teamId = this.team!.id;
    this.projectService.createProject(createProjectDto).subscribe({
      next: (project: ProjectDto) => {
        this.team?.projects.push(project);
        this.toggleWindow(false);
      },
    });
  }

  onEdit(createTeamDto: CreateTeamDto): void {
    this.teamService.updateTeam(createTeamDto, this.team!.id).subscribe({
      next: () => {
        this.team = {
          // merge if successful
          ...(this.team as any),
          ...createTeamDto,
        };
        this.toggleWindow(false);
      },
    });
  }

  onDelete(): void {
    this.teamService.deleteTeam(this.team!.id).subscribe({
      next: () => {
        this.router.navigate(['teams']);
      },
    });
  }

  onKick(memberId: number): void {
    this.teamService.kickMember(this.team!.id, memberId).subscribe(() => {
      this.team.members = this.team.members.filter(
        (member) => member.id !== memberId,
      );
      this.toggleWindow(false);
    });
  }

  onAddMember(username: string): void {
    this.teamService.addMember(this.team!.id, username).subscribe(() => {
      window.location.reload();
    });
  }

  onUpdateRole(updateRoleDto: UpdateRoleDto): void {
    this.teamService
      .updateMemberRole(this.team!.id, updateRoleDto)
      .subscribe(() => {
        this.selectedMember!.permission = updateRoleDto.role;
        this.toggleWindow(false);
      });
  }

  openEditWindow(): void {
    this.toggleWindow(true, this.editTeamTemplate);
  }

  openDeleteWindow(): void {
    this.toggleWindow(true, this.deleteTeamTemplate);
  }

  openKickMemberWindow(member: MemberDto): void {
    this.selectedMember = member;
    this.toggleWindow(true, this.kickMemberTemplate);
  }

  openAddMemberWindow(): void {
    this.toggleWindow(true, this.addMemberTemplate);
  }

  openUpdateRoleWindow(member: MemberDto): void {
    this.selectedMember = member;
    this.toggleWindow(true, this.updateRoleTemplate);
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
    }
  }

  showKickButton(member: MemberDto): Observable<boolean> {
    return new Observable<boolean>((observer) => {
      this.userId?.subscribe({
        next: (id: number) => {
          observer.next(
            this.team?.userPermission === MemberPermission.ADMIN &&
              member.id !== id,
          );
        },
      });
    });
  }

  isUserAdmin(): Observable<boolean> {
    return new Observable<boolean>((observer) => {
      this.isAdmin?.subscribe({
        next: (value: boolean) => {
          observer.next(
            this.team?.userPermission === MemberPermission.ADMIN || value,
          );
        },
      });
    });
  }

  canUserModify(): Observable<boolean> {
    return new Observable<boolean>((observer) => {
      this.isAdmin?.subscribe({
        next: (value: boolean) => {
          observer.next(
            this.team?.userPermission === MemberPermission.READ_WRITE || value,
          );
        },
      });
    });
  }
}
