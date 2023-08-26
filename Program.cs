using TaskMgmt.Context;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Auth;

TaskContext context = new TaskContext();
Authentication auth = new Authentication();
auth.ShowOptions();
