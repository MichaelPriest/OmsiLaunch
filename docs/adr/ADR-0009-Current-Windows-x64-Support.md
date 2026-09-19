# ADR-0009: Current Windows x64 Support

Decision: OmsiLaunch Current officially supports only final/latest serviced
Windows 10 x86-64 and current Windows 11 x86-64. The outer Current host is
x64; OMSI and its in-process runtime remain x86.

Reason: this keeps the production support matrix deliberately narrow,
reproducible, and testable. Historical Windows support is handled through
separate Legacy ports rather than weakening Current guarantees.
