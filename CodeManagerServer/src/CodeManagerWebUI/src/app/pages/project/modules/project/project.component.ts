import {
  Component,
  Input,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import {
  CreateProjectDto,
  ProjectDataDto,
} from '../../../../services/project/project.interface';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../../../services/project/project.service';
import { MemberPermission } from '../../../../services/team/team.interface';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';
import { RunService } from '../../../../services/run/run.service';
import { State } from '../../../../services/run/run.interface';
import { SignalRService } from '../../../../services/signalr/signalr.service';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss'],
})
export class ProjectComponent implements OnInit {
  currentTemplate?: TemplateRef<any>;

  State = State;

  @ViewChild('editProjectTemplate') editProjectTemplate?: TemplateRef<any>;
  @ViewChild('deleteProjectTemplate') deleteProjectTemplate?: TemplateRef<any>;
  @ViewChild('startRunTemplate') startRunTemplate?: TemplateRef<any>;

  @Input() projectData?: ProjectDataDto;
  showWindow: boolean = false;

  @Select((state: any) => state.app.user.user.isAdmin)
  isAdmin?: Observable<boolean>;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly projectService: ProjectService,
    private readonly signalrService: SignalRService,
    readonly runService: RunService,
  ) {}

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.projectService
      .getProject(id)
      .subscribe((projectData: ProjectDataDto) => {
        this.projectData = projectData;
        // TODO: join group
        this.signalrService.registerMethod(
          'ReceiveStepResultEvent',
          this.onReceiveStepResultEvent,
        );
      });
  }

  openDeleteWindow(): void {
    this.toggleWindow(true, this.deleteProjectTemplate);
  }

  openEditWindow(): void {
    this.toggleWindow(true, this.editProjectTemplate);
  }

  openStartRunWindow(): void {
    this.toggleWindow(true, this.startRunTemplate);
  }

  onEdit(createProjectDto: CreateProjectDto): void {
    this.projectService
      .updateProject(createProjectDto, this.projectData!.project.id)
      .subscribe(() => {
        this.projectData = {
          ...(this.projectData as any),
          project: {
            ...(this.projectData?.project as any),
            isPrivateProject: createProjectDto.isPrivateProject,
            description: createProjectDto.description,
            name: createProjectDto.name,
          },
          repositoryUrl: createProjectDto.repositoryUrl,
        };
        this.toggleWindow(false);
      });
  }

  onDelete(): void {
    this.projectService.deleteProject(this.projectData!.project.id).subscribe({
      next: () => {
        this.router.navigate(['projects']);
      },
    });
  }

  onStart(file: File | null): void {
    if (file) {
      this.runService
        .startRun(this.projectData!.project.id, file)
        .subscribe(() => this.toggleWindow(false));
    }
  }

  onReceiveStepResultEvent(stepResultEvent: any): void {
    console.log('step result', stepResultEvent);
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
    }
  }

  async openRun(id: number): Promise<void> {
    await this.router.navigate([`runs/${id}`]);
  }

  showActions(): Observable<boolean> {
    return new Observable<boolean>((observer) => {
      this.isAdmin?.subscribe({
        next: (value: boolean) => {
          observer.next(
            value ||
              this.projectData?.userPermission === MemberPermission.ADMIN ||
              this.projectData?.userPermission === MemberPermission.READ_WRITE,
          );
        },
      });
    });
  }
}
