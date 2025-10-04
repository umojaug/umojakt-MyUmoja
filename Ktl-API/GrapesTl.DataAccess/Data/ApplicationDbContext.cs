﻿using GrapesTl.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GrapesTl.Service;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    //Iron Man
    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<UserOtp> UserOtp { get; set; }


    //Basic Setup Start
    public DbSet<Expense> Expense { get; set; }

    //Basic Setup End

}


